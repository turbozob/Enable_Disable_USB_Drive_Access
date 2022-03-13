using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32; // registry dependency
using System.Reflection; // for version info
using System.Diagnostics;// for version info

namespace Enable_Disable_USB_Drive_Access
{

    public partial class Form1 : Form
    {
        const string userRoot = "HKEY_CURRENT_USER";
        const string subkey = "RegistrySetValueExample";
        const string keyName = "Start";
        Boolean usbAccessControl = false;
        Boolean usbAccessState = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // get app version info
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            lblVersion.Text = "v" + fileVersionInfo.ProductVersion;

            if (usbAccessStatusMethod() == true)
            {
                //txtUsbStatus.BackColor = Color.LimeGreen;
                //txtUsbStatus.Font = new Font(txtUsbStatus.Font, FontStyle.Bold);
                //txtUsbStatus.Text = "Enabled";
            }
            else
            {
                //txtUsbStatus.BackColor = Color.Yellow;
                //txtUsbStatus.Font = new Font(txtUsbStatus.Font, FontStyle.Regular);
                //txtUsbStatus.Text = "Disabled";
            }
        }

        private void btnEnableUsbAccess_Click(object sender, EventArgs e)
        {
            // check current usb access status
            if (usbAccessStatusMethod() == true)   
            {
                usbAccessControl = false;  // if currently enabled, then disable
            }
            else
            {
                usbAccessControl = true;    // if currently disabled, then enable
            }

            // execute usb access control enable disable
            usbAccessControlMethod(usbAccessControl);
            // after change check status
            usbAccessStatusMethod();

        }
        private bool usbAccessStatusMethod()
        {

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\\CurrentControlSet\\Services\\UsbStor"))
                {
                    if (key != null)
                    {
                        Object usbStateInt = key.GetValue(keyName);
                        if (usbStateInt != null)
                        {
                            switch (usbStateInt) // rvk=registry value kind
                            {
                                case 3:
                                    usbAccessState = true;
                                    break;

                                case 4:
                                    usbAccessState = false;
                                    break;
                            }

                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                usbAccessState=false;
            }

            // return value
            if (usbAccessState)
            {
                txtUsbStatus.BackColor = Color.LimeGreen;
                txtUsbStatus.Font = new Font(txtUsbStatus.Font, FontStyle.Bold);
                txtUsbStatus.Text = "Enabled";
                btnEnableUsbAccess.Text = "Disable";
                lblStatus.Text = "USB Access Enabled";
                return true;
            }
            else
            {
                txtUsbStatus.BackColor = Color.Yellow;
                txtUsbStatus.Font = new Font(txtUsbStatus.Font, FontStyle.Regular);
                txtUsbStatus.Text = "Disabled";
                btnEnableUsbAccess.Text = "Enable";
                lblStatus.Text = "USB Access Disabled";
                return false;
            }
           
        }
        private void usbAccessControlMethod(bool enable)
        {
            try
            {
                using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\\CurrentControlSet\\Services\\UsbStor"))
                {
                    //if (registryKey != null)
                    //{
                    //registryKey.SetValue("Deno", "1", RegistryValueKind.String);
                    //registryKey.Close();
                    //}


                    if (enable)
                    {
                    registryKey.SetValue("Start", 3, RegistryValueKind.DWord);
                    }
                    else
                    {
                    registryKey.SetValue("Start", 4, RegistryValueKind.DWord);
                    }
                } // end using
            } // end try
            catch (Exception ex)
            {
                if (enable)
                {
                    MessageBox.Show("Could not enable USB drive access.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    MessageBox.Show("Could not disable USB drive access.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            } // end catch


        }



    }
}
