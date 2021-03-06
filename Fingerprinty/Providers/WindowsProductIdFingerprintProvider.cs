﻿using System;
using Microsoft.Win32;

namespace Fingerprinty
{
    /// <summary>
    /// Calculates <see cref="Fingerprint"/> based on the Windows ProductId.
    /// The ProductId is generated by Windows during system installation.
    /// </summary>
    public class WindowsProductIdFingerprintProvider : FingerprintProvider
    {
        /// <inheritdoc />
        public override SupportedPlatforms SupportedPlatforms { get; } = SupportedPlatforms.Windows;

        /// <inheritdoc />
        public WindowsProductIdFingerprintProvider(FingerprintFactory fingerprintFactory) : base(fingerprintFactory)
        {
        }

        /// <inheritdoc />
        public override Fingerprint Get()
        {
            var productId = GetProductId();

            return FingerprintFactory.Create(productId);
        }
        
        private static string GetProductId()
        {
            string productId;

            try
            {
                using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var windowsNtKey = localMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion"))
                {
                    productId = windowsNtKey?.GetValue("ProductId") as string;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to calculate ProductId.", ex);
            }

            if (productId == null) throw new InvalidOperationException("ProductId is null.");

            return productId;
        }
    }
}