

void Main()
{
    string email = "nulled.to";
    string product_identity = "1";
    string product_edition = "3"; // 1 - Trial, 2 - ??, 3 - Ultimate
    DateTime expire_date = new DateTime(2099, 12, 31);
    string license_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GunaFramework");
    string license_file = Path.Combine(license_dir, "GunaUI2.cdm");
 
    if(!Directory.Exists(license_dir)) Directory.CreateDirectory(license_dir);
 
    var license = new License();
 
    string license_data = string.Join("~", new string[] { product_identity, product_edition, expire_date.ToString("yyyy-MM-dd") });
 
    string activation_code = license.GenActivationCode("ilham", email, license_data);
 
    string machine_id = License.GetMachineID();
 
    string activation_data = string.Join(",", new string[] { email, activation_code, machine_id });
 
    string enc_activation_data = license.Encrypt(activation_data);
 
    File.WriteAllText(license_file, enc_activation_data);
 
    email.Dump("EMAIL:");
    activation_code.Dump("ACTIVATION CODE:");
    activation_data.Dump("PLAIN ACTIVATION DATA:");
    enc_activation_data.Dump("ENC ACTIVATION DATA:");
    license_file.Dump("LICENSE FILE:");
}
 
public class License
{
    string string_7 = "P@@Sw0rd";
    string string_8 = "S@LT&KEY";
    string string_9 = "@1B2c3D4e5F6g7H8";
 
    public string GenActivationCode(string salt, string email, string license_data)
    {
        string text = this.CalcHash(salt + email);
        string text2 = string.Empty;
        checked
        {
            for (int i = 1; i <= license_data.Length; i++)
            {
                text2 += (Strings.Asc(license_data[i - 1]) ^ unchecked((long)Strings.Asc(text[i % text.Length]))).ToString("X2");
            }
            return text2;
        }
    }
 
    string CalcHash(string string_10)
    {
        HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
        byte[] array = Encoding.ASCII.GetBytes(string_10);
        array = hashAlgorithm.ComputeHash(array);
        string text = "";
        foreach (byte b in array)
        {
            text += b.ToString("x2");
        }
        return text;
    }
 
    public static string GetMachineID()
    {
        string result;
        try
        {
            result = Conversions.ToString(Operators.AddObject(Conversions.ToString(Operators.AddObject("", Registry.GetValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", "ProcessorNameString", "??"))), Registry.GetValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\BIOS", "SystemManufacturer", "")));
        }
        catch (Exception ex)
        {
            result = "??";
        }
        return result;
    }
 
    public string Encrypt(string string_10)
    {
        string result;
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(string_10);
            byte[] bytes2 = new Rfc2898DeriveBytes(this.string_7, Encoding.ASCII.GetBytes(this.string_8)).GetBytes(32);
            ICryptoTransform transform = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros
            }.CreateEncryptor(bytes2, Encoding.ASCII.GetBytes(this.string_9));
            byte[] inArray;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                    inArray = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            result = Convert.ToBase64String(inArray);
        }
        catch
        {
            result = "";
        }
        return result;
    }
}
