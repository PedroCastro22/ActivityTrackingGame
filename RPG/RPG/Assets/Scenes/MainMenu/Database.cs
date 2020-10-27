using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;

public class Database : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject connectPanel;
    [SerializeField] private GameObject signUpPanel;

    [SerializeField] private Text helloNickname;

    [Header("Firebase")]
    [SerializeField] private DependencyStatus dependencyStatus;
    [SerializeField] private FirebaseAuth auth;
    [SerializeField] private FirebaseUser user; 

    [Header("Login")]
    [SerializeField] private InputField loginEmail;
    [SerializeField] private InputField loginPassword;
    [SerializeField] private Text loginErrorMessage;

    [Header("Register")]
    [SerializeField] private InputField signupEmail;
    [SerializeField] private InputField signupPassword;
    [SerializeField] private InputField signupConfirmPassword;
    [SerializeField] private InputField signupNickname;
    [SerializeField] private Text signupErrorMessage;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
    }

    public void OnLogin()
    {
        StartCoroutine(Login(loginEmail.text, loginPassword.text));
    }

    public void OnSignUp()
    {
        StartCoroutine(SignUp(signupEmail.text, signupPassword.text, signupNickname.text));
    }

    private IEnumerator Login(string _email, string _password)
    {
        //loginErrorMessage.text = "Here";
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        loginErrorMessage.text = "Here";
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
        loginErrorMessage.text = "Here2";

        if (LoginTask.Exception != null)
        {
            loginErrorMessage.text = "Here3";
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            loginErrorMessage.text = message;
        }
        else
        {
            loginErrorMessage.text = "Here4";
            //User is now logged in
            //Now get the result
            user = LoginTask.Result;
            helloNickname.text = "Hello " + user.DisplayName;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
            loginErrorMessage.text = "";
            OnLoginClicked();
        }
    }

    private IEnumerator SignUp(string _email, string _password, string _nickname)
    {
        if (_nickname == "")
        {
            //If the username field is blank show a warning
            signupErrorMessage.text = "Missing Nickname";
        }
        else if (signupPassword.text != signupConfirmPassword.text)
        {
            //If the password does not match show a warning
            signupErrorMessage.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                signupErrorMessage.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                user = RegisterTask.Result;

                if (user != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _nickname };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = user.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        signupErrorMessage.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        OnGoBackClicked();
                        signupErrorMessage.text = "";
                    }
                }
            }
        }
    }


    public void OnSignUpClicked()
    {
        loginPanel.SetActive(false);
        signUpPanel.SetActive(true);
    }

    public void OnGoBackClicked()
    {
        loginPanel.SetActive(true);
        signUpPanel.SetActive(false);
    }

    public void OnLoginClicked()
    {
        connectPanel.SetActive(true);
        loginPanel.SetActive(false);
    }
        
}
