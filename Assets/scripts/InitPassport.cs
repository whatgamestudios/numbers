using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Immutable.Passport;
  
public class InitPassport : MonoBehaviour
{
    private Passport passport;
    
    async void Start()
    {
        // Replace with your actual Passport Client ID
        string clientId = "N5pi7DdS7xCeGFoQKFinU6sEY8f8NPuh";
        
        // Set the environment to SANDBOX for testing or PRODUCTION for production
        string environment = Immutable.Passport.Model.Environment.PRODUCTION;
        
        // Initialise Passport
        passport = await Passport.Init(clientId, environment);

        login();

    }

    async private void login() {
        Debug.Log("Logged in: " + PassportStore.IsLoggedIn());
        Debug.Log("Credentials saved: " + await Passport.Instance.HasCredentialsSaved());
        // Check if the player is supposed to be logged in and if there are credentials saved
        if (PassportStore.IsLoggedIn() && await Passport.Instance.HasCredentialsSaved()) {
            // Try to log in using saved credentials
            bool success = await Passport.Instance.Login(useCachedSession: true);
            Debug.Log("Success: " + success);
            // Update the login flag
            PassportStore.SetLoggedIn();
        } else {
            // No saved credentials to re-login the player, login
            await passport.LoginPKCE();
            Debug.Log("LoginPKCE done");
            Debug.Log("Credentials saved2: " + await Passport.Instance.HasCredentialsSaved());

            if (await Passport.Instance.HasCredentialsSaved()) {
                PassportStore.SetLoggedIn();
            }
        }
    }
}