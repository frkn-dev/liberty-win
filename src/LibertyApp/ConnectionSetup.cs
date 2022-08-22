using LibertyApp.Language;
using LibertyApp.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace LibertyApp;

/// <summary>
/// Configure IKEv2 VPN clients
/// <see href="https://github.com/hwdsl2/setup-ipsec-vpn/blob/master/docs/ikev2-howto.md#windows-7-8-10-and-11"/>
/// </summary>
public static class ConnectionSetup
{
	/// <summary>
	/// Import .p12 file
	/// </summary>
	public static void ImportPfx()
	{
		try
		{
			using var storePersonal = new X509Store(StoreName.My, StoreLocation.LocalMachine, OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);
			using var storeTrustedRoot = new X509Store(StoreName.Root, StoreLocation.LocalMachine, OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);

			if (!storePersonal.Certificates.Find(X509FindType.FindByThumbprint, Resources.ThumbPrintVpnClient, false).Any()
				&& !storeTrustedRoot.Certificates.Find(X509FindType.FindByThumbprint, Resources.ThumbPrintIkeV2VpnCa, false).Any())
			{
				if (MessageBox.Show(Strings.NeedForEncryptionKeysMessage,
					Strings.NeedForEncryptionKeys,
					MessageBoxButton.YesNo,
					MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					if (!File.Exists(Resources.KeysFilePath))
					{
						storePersonal.Close();
						storeTrustedRoot.Close();

						throw new FileNotFoundException(Strings.EncryptionKeysFileNotFound, Resources.KeysFilePath);
					}

					using var process = new Process
					{
						StartInfo = new ProcessStartInfo("cmd.exe")
						{
							ArgumentList = {
								"/c",
								"certutil",
								"-f",
								"-p",
								"",
								"-importpfx",
								Resources.KeysFilePath,
								"NoExport"
							},
							WorkingDirectory = Environment.CurrentDirectory,
							UseShellExecute = false,
							CreateNoWindow = true,
						}
					};

					process.Start();
					process.WaitForExit();

					switch (process.ExitCode)
					{
						case 0:
							break;

						default:
							MessageBox.Show($"Error: {process.ExitCode}", "CertUtil Error");
							storePersonal.Close();
							storeTrustedRoot.Close();
							App.Current.Shutdown();
							break;
					}

					CreateConnection();
					SettingIpSecConfiguration();
				}
				else
				{
					App.Current.Shutdown();
				}
			}

			storePersonal.Close();
			storeTrustedRoot.Close();
		}
		catch (Exception e)
		{
			if (MessageBox.Show($"{e.Message}\n\n{Strings.ApplicationWillBeClosed}",
				Strings.ErrorImportEcryptionKeys,
				MessageBoxButton.OK,
				MessageBoxImage.Error,
				MessageBoxResult.OK,
				MessageBoxOptions.None) == MessageBoxResult.OK)
			{
				App.Current.Shutdown();
			}
		}
	}

	/// <summary>
	/// Create VPN connection
	/// </summary>
	public static void CreateConnection()
	{
		using var process = new Process
		{
			StartInfo = new ProcessStartInfo("cmd.exe")
			{
				ArgumentList = {
						"/c",
						"powershell",
						"Add-VpnConnection",
						$"-ServerAddress {Resources.ServerName}",
						$"-Name {Resources.ConnectionName}",
						"-TunnelType IKEv2",
						"-AuthenticationMethod MachineCertificate",
						"-EncryptionLevel Required",
						"-PassThru",
					},
				WorkingDirectory = Environment.CurrentDirectory,
				UseShellExecute = false,
				CreateNoWindow = true,
			}
		};

		process.Start();
		process.WaitForExit();
	}

	/// <summary>
	/// Set IPSec configuration
	/// </summary>
	public static void SettingIpSecConfiguration()
	{
		using var process = new Process
		{
			StartInfo = new ProcessStartInfo("cmd.exe")
			{
				ArgumentList = {
					"/c",
					"powershell",
					"Set-VpnConnectionIPsecConfiguration",
					$"-ConnectionName {Resources.ConnectionName}",
					"-AuthenticationTransformConstants GCMAES128",
					"-CipherTransformConstants GCMAES128",
					"-EncryptionMethod AES256",
					"-IntegrityCheckMethod SHA256",
					"-PfsGroup None",
					"-DHGroup Group14",
					"-PassThru",
					"-Force",
				},
				WorkingDirectory = Environment.CurrentDirectory,
				UseShellExecute = false,
				CreateNoWindow = true,
			}
		};

		process.Start();
		process.WaitForExit();
	}

	/// <summary>
	/// Disconnect
	/// </summary>
	public static void Disconnect()
	{
		using var process = new Process
		{
			StartInfo = new ProcessStartInfo("cmd.exe")
			{
				WorkingDirectory = Environment.CurrentDirectory,
				UseShellExecute = false,
				CreateNoWindow = true,
				ArgumentList = {
					"/c",
					"rasdial",
					"/d"
				},
			},
		};

		process.Start();
		process.WaitForExit();
	}
}