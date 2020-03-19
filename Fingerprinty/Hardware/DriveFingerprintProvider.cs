﻿using System;
using System.Linq;
using System.Management;
using Mono.Unix;

namespace Fingerprinty.Hardware
{
    public class DriveFingerprintProvider : HardwareFingerprintProvider
    {
        public override SupportedPlatforms SupportedPlatforms { get; } = SupportedPlatforms.Windows;

        public override HardwareFingerprint Get()
        {
            var cDriveSerial = GetCDriveSerial();

            return FingerprintFactory.Create(cDriveSerial);
        }

        private static string GetCDriveSerial()
        {
            try
            {
                string volumeSerial;
                using (var disk = new ManagementObject(@"win32_logicaldisk.deviceid=""C:"""))
                {
                    disk.Get();
                    volumeSerial = disk["VolumeSerialNumber"].ToString();
                    disk.Dispose();
                }

                return volumeSerial;
            }
            catch (Exception ex)
            {
                throw new FingerprintyException("Not possible to find disk C:.", ex);
            }
        }
    }
}