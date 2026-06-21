using System;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace OA2CertTool
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetSystemFirmwareTable(uint provider, uint tableId, IntPtr buffer, uint bufferSize);

        // 0x41435049 == 'ACPI', 0x43494C53 = 'CILS' = 'SLIC' in little endian
        private const uint ACPI_ID = 0x41435049;
        private const uint SLIC_ID = 0x43494C53;
        private string dataCrc;
        private byte[] cert;
        public static byte[] GetSLICTable()
        {
            if (GetSystemFirmwareTable(ACPI_ID, SLIC_ID, IntPtr.Zero, 0) != 374)
            {
                throw new Exception("ACPI SLIC table not found or has invalid length!");
            }

            IntPtr buffer = Marshal.AllocHGlobal(374);
            if (GetSystemFirmwareTable(ACPI_ID, SLIC_ID, buffer, 374) == 0)
            {
                throw new Exception("Failed to read ACPI SLIC table!");
            }

            byte[] data = new byte[374];
            Marshal.Copy(buffer, data, 0, 374);

            return data;
        }
        public static bool ValidateChecksum(byte[] acpiTable)
        {
            byte sum = 0;
            for (int i = 0; i < acpiTable.Length; i++) sum += acpiTable[i];
            return sum == 0;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                byte[] SLIC = GetSLICTable();

                Crc32 crc = new Crc32();
                uint crc32 = crc.Append(0, SLIC, 10, 6);
                crc32 = crc.Append(crc32, SLIC, 60, 132);
                dataCrc = crc32.ToString("X8");

                lblOemID.Text = Encoding.ASCII.GetString(SLIC, 10, 6);
                lblTableID.Text = Encoding.ASCII.GetString(SLIC, 16, 8);
                lblDataCRC.Text = dataCrc;
                lblRevision.Text = BitConverter.ToUInt32(SLIC, 24).ToString();
                lblCreator.Text = Encoding.ASCII.GetString(SLIC, 28, 4);
                lblCreatorRev.Text = BitConverter.ToUInt32(SLIC, 32).ToString();
                lblOAVersion.Text = BitConverter.ToUInt16(SLIC, 228).ToString() + "." + BitConverter.ToUInt16(SLIC, 226).ToString();

                if (!ValidateChecksum(SLIC))
                {
                    lblChecksum.Text = "Invalid";
                    lblChecksum.ForeColor = Color.Crimson;
                }
                else
                {
                    lblChecksum.Text = "OK";
                    lblChecksum.ForeColor = Color.ForestGreen;
                }

                cert = (byte[])Properties.Resources.ResourceManager.GetObject(dataCrc);
                if (cert == null)
                {
                    button1.Enabled = false;
                    button2.Enabled = false;
                    lblCertStatus.Text = "Not Found";
                    lblCertStatus.ForeColor = Color.Crimson;
                }
                else
                {
                    lblCertStatus.Text = "Found";
                    lblCertStatus.ForeColor = Color.ForestGreen;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ManagementClass SPP = new ManagementClass("SoftwareLicensingService");
                ManagementObjectCollection instances = SPP.GetInstances();
                if (instances.Count != 1)
                {
                    MessageBox.Show("More than one SPP instance running!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (ManagementObject instance in instances)
                {
                    instance.InvokeMethod("InstallLicense", new[] { Encoding.ASCII.GetString(cert) });
                }
                MessageBox.Show("Certificate installed!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            System.IO.Stream s;
            d.Filter = "XrML Digital License (*.xrm-ms)|*.xrm-ms";
            if (d.ShowDialog() == DialogResult.OK)
            {
                if ((s = d.OpenFile()) != null)
                {
                    s.Write(cert, 0, cert.Length);
                    s.Close();
                    MessageBox.Show("Certificate saved!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else
                {
                    MessageBox.Show("Failed to save file!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
