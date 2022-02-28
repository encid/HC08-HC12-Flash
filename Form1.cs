using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HC08_HC12_Flash
{
    public partial class Form1 : Form
    {
        public string VAULT_PATH = @"V:\Released_Part_Information\240-xxxxx-xx_Software\";
        public string TEMP_PATH = @"C:\Temp\HC12Prog";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private bool CopySoftwareFilesToTemp(string softwarePartNumber)
        {
            var configFile = GetFilePath(softwarePartNumber, ".cfg");
            var s19File = GetFilePath(softwarePartNumber, ".s19");
            File.Copy(configFile, $@"{TEMP_PATH}\{softwarePartNumber}.cfg");
            File.Copy(s19File, $@"{TEMP_PATH}\{softwarePartNumber}.s19");
            return true;
        }

        private bool EditConfigFile(string softwarePartNumber, string configFile)
        {
            string[] lines = File.ReadAllLines(configFile);

            string lineToFind = lines.

            lines[0] = $"SS {softwarePartNumber}.s19";
            File.WriteAllLines(configFile, lines);
            return true;
        }

        private string GetFilePath(string softwarePartNumber, string extension)
        {
            // make sure it is a properly formatted software part number
            Regex regex = new Regex("240-9\\d{4}-\\d\\d", RegexOptions.IgnoreCase);
            
            if (!regex.IsMatch(softwarePartNumber))
            {
                return string.Empty;
            }

            var softwareECLDir = GetSoftwareVaultPath(softwarePartNumber);
            var dir = new DirectoryInfo(GetSoftwareVaultPath(softwarePartNumber));
            var fileList = dir.GetFiles().Where(file => file.Extension.ToLower() == extension);

            var configFile = fileList.FirstOrDefault();

            return configFile.FullName;
        }

        private string GetSoftwareVaultPath(string softwarePartNumber)
        {
            // make sure it is a properly formatted software part number
            Regex regex = new Regex("240-9\\d{4}-\\d\\d", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(softwarePartNumber))
            {
                return string.Empty;
            }

            var softwarePartOne = softwarePartNumber.Substring(4, 5);
            string eclDir;

            //set software vault path to appropriate number depending on what the software pn starts with
            string softwarePrePath = $@"240-9XXXX-XX\240-{softwarePartOne.Substring(0, 2)}XXX-XX";

            try
            {
                var softwareBaseDir = Directory.GetDirectories($@"{VAULT_PATH}\{softwarePrePath}\240-{softwarePartOne}-XX\")
                                           .FirstOrDefault(dir => dir.Contains(softwarePartNumber));

                eclDir = Directory.GetDirectories(softwareBaseDir).FirstOrDefault(dir => dir.Contains("ECL"));
            }
            catch (Exception e)
            {
                //Logger.Log(e.Message, rt, System.Drawing.Color.Red);
                return string.Empty;
            }

            return eclDir;
        }
    

        private string GetLatestECL(string softwarePartNumber)
        {
            // make sure it is a properly formatted software part number
            Regex regex = new Regex("240-9\\d{4}-\\d\\d", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(softwarePartNumber))
            {
                return string.Empty;
            }

            // get software path
            var softwarePath = GetSoftwareVaultPath(softwarePartNumber);

            var eclString = softwarePath.Substring(softwarePath.Length - 6);
            var dashIndex = eclString.IndexOf("-", StringComparison.CurrentCulture);
                        
            return eclString.Substring(dashIndex + 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            // check text file to see if current rev is already copied into temp folder

            textBox2.Text = GetFilePath(textBox1.Text, ".s19");
            CopySoftwareFilesToTemp(textBox1.Text);
            EditConfigFile(textBox1.Text, $@"{TEMP_PATH}\{textBox1.Text}.cfg");
        }
    }
}
