using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AdventOfCode.Utils
{
  class InputGrabber
  {
    public static bool TryFetchInput(int year, int day)
    {
      DateTime est = DateTime.Now.ToUniversalTime().AddHours(-5);

      try
      {
        if (est >= new DateTime(year, 12, day))
        {
          string cookie = string.Empty;

          // Check the config file for a cookie first
          if (IOUtils.ConfigObject().ContainsKey("cookie"))
          {
            cookie = (string)IOUtils.ConfigObject()["cookie"];
          }

          // Otherwise, check cached cookie data
          if (!cookie.Equals(string.Empty) || TryGetSessionCookie(out cookie))
          {
            WebClient client = new WebClient();
            client.Headers.Add(HttpRequestHeader.Cookie, cookie);
            File.WriteAllText(IOUtils.InputPath(year, day), client.DownloadString(IOUtils.InputURL(year, day)));
          }
          else
          {
            Console.Error.WriteLine("Please add cookie to config.json, or log into Advent of Code in Chrome, and retry.");
            throw new Exception();
          }
        }
        else
        {
          Console.Error.WriteLine("Can't request input before the puzzle is released!");
          throw new Exception();
        }
      }
      catch (Exception)
      {
        Console.Error.WriteLine("Unable to retrieve input.");
        return false;
      }

      return true;
    }

    private static bool TryGetSessionCookie(out string decryptedData)
    {
      decryptedData = string.Empty;
      bool dataFound = false;
      var dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Cookies";
      if (!File.Exists(dbPath)) throw new FileNotFoundException("Cookies not found", dbPath);

      var connectionString = "Data Source=" + dbPath + ";pooling=false";
      var connection = new System.Data.SQLite.SQLiteConnection(connectionString);
      var command = connection.CreateCommand();

      command.CommandText = "SELECT name,encrypted_value FROM cookies WHERE host_key = '.adventofcode.com'";

      connection.Open();
      var reader = command.ExecuteReader();

      while (reader.Read())
      {
        if (reader.GetString(0).StartsWith("session"))
        {
          var encryptedData = (byte[])reader[1];

          string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
          string encKey = File.ReadAllText(localAppDataPath + @"\Google\Chrome\User Data\Local State");
          encKey = JObject.Parse(encKey)["os_crypt"]["encrypted_key"].ToString();
          var decodedKey = System.Security.Cryptography.ProtectedData.Unprotect(Convert.FromBase64String(encKey).Skip(5).ToArray(), null, System.Security.Cryptography.DataProtectionScope.LocalMachine);
          decryptedData = "session=" + _decryptWithKey(encryptedData, decodedKey, 3);
          dataFound = true;
        }
      }

      connection.Close();
      return dataFound;
    }

    private static string _decryptWithKey(byte[] message, byte[] key, int nonSecretPayloadLength)
    {
      const int KEY_BIT_SIZE = 256;
      const int MAC_BIT_SIZE = 128;
      const int NONCE_BIT_SIZE = 96;

      if (key == null || key.Length != KEY_BIT_SIZE / 8)
        throw new ArgumentException(String.Format("Key needs to be {0} bit!", KEY_BIT_SIZE), "key");
      if (message == null || message.Length == 0)
        throw new ArgumentException("Message required!", "message");

      using (var cipherStream = new MemoryStream(message))
      using (var cipherReader = new BinaryReader(cipherStream))
      {
        var nonSecretPayload = cipherReader.ReadBytes(nonSecretPayloadLength);
        var nonce = cipherReader.ReadBytes(NONCE_BIT_SIZE / 8);
        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(key), MAC_BIT_SIZE, nonce);
        cipher.Init(false, parameters);
        var cipherText = cipherReader.ReadBytes(message.Length);
        var plainText = new byte[cipher.GetOutputSize(cipherText.Length)];
        try
        {
          var len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
          cipher.DoFinal(plainText, len);
        }
        catch (InvalidCipherTextException)
        {
          return null;
        }
        return Encoding.Default.GetString(plainText);
      }
    }
  }
}
