using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RundApp.UserInterface;
using AppNetDotNet.ApiCalls;
using AppNetDotNet.Model;

namespace RundApp
{
    public class AppController
    {
        public static AppController Current;
        public MainWindow mainwindow;

        // enter your app details below
        private string client_id = "Enter your one...";
        private string redirect_uri = "http://www.li-ghun.de/oauth/";
        private string scope = "basic stream write_post follow messages files update_profile";

        public static string access_token { get; set; }
        public Token token { get; set; }
        public User user { get; set; }

        public static void Start()
        {
            // We'll vreate a Singleton to be available all the time regardless on which windows are open
            if (Current == null)
            {
                Current = new AppController();
            }
        }

        private AppController()
        {
            Current = this;

            // the authorization is done using an embedded Internet Explorer control - which is in quirks mode by default
            // so the library has a method to tell Windows this app is fine to use a real browser mode (you can choose which one - IE 9 in this case)
            Authorization.registerAppInRegistry(Authorization.registerBrowserEmulationValue.IE9Always, alsoCreateVshostEntry: true);

            #region Check for update and upgrade settings if so
            try
            {
                if (!Properties.Settings.Default.settings_updated)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.settings_updated = true;
                }
            }
            catch
            {
                try
                {
                    Properties.Settings.Default.Reset();
                }
                catch { }
            }
            #endregion

            // note: you must not store the access_token in plain text. Add an encryption here!
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.access_token))
            {
                authorize_account();
            }
            else
            {
                // the access token should be stored encrypted!
                // it is not here in this example...
                access_token = Properties.Settings.Default.access_token;
                check_access_token();
            }
        }

        private void authorize_account()
        {
            // will open a new window asking the user to login and authorize your app
            // btw: this window will log out the user first which is handy if you want to support multiple accounts...
            AppNetDotNet.Model.Authorization.clientSideFlow apnClientAuthProcess = new Authorization.clientSideFlow(client_id, redirect_uri, scope);
            apnClientAuthProcess.AuthSuccess += apnClientAuthProcess_AuthSuccess;
            apnClientAuthProcess.showAuthWindow();
        }

        private bool check_access_token()
        {
            if(!string.IsNullOrWhiteSpace(access_token)) {
                Tuple<Token, ApiCallResponse> response = Tokens.get(access_token);
                if (response.Item2.success)
                {
                    token = response.Item1;
                    user = token.user;
                    Properties.Settings.Default.access_token = access_token;
                    open_main_window();

                    return true;
                }
            }
            // not authorized successfully - retrying
            authorize_account();
            return false;
        }

        void apnClientAuthProcess_AuthSuccess(object sender, AppNetDotNet.AuthorizationWindow.AuthEventArgs e)
        {
            if (e != null)
            {
                if (e.success)
                {
                    access_token = e.accessToken;
                    check_access_token();
                }
            }
        }

        private void open_main_window()
        {
            mainwindow = new MainWindow();
            mainwindow.Show();
        }
        
    }
}
