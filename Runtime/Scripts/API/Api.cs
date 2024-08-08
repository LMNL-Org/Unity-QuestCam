using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BRIJ.Code.Models;
using BRIJ.Models;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;

namespace BRIJ
{
    public class Api
    {
        //private static readonly string BaseUrl = "http://127.0.0.1";
        private static readonly string BaseUrl = "https://api.brij.social";
        private static readonly string SignUpUrl = BaseUrl + "/register";
        private static readonly string NewSessionUrl = BaseUrl + "/gapi/session/login";
        private static readonly string GetPostsUrl = BaseUrl + "/gapi/post/list";
        private static readonly string ReactUrl = BaseUrl + "/gapi/post/react";
        private static readonly string GetImageUrl = BaseUrl + "/gapi/image/";
        public static readonly string GetVideoUrl = BaseUrl + "/gapi/video/";
        private static readonly string VerifySessionUrl = BaseUrl + "/gapi/session/verify";
        private static readonly string RestoreSessionUrl = BaseUrl + "/gapi/session/restore";
        private static readonly string LogoutSessionUrl = BaseUrl + "/gapi/session/logout";
        private static readonly string LoginWithMetaUrl = BaseUrl + "/gapi/session/meta_login";
        private static readonly string UploadPostUrl = BaseUrl + "/gapi/post/new";
        
        
        private static readonly string RequestStreamURL = BaseUrl + "/gapi/streaming/request";
        private static readonly string CheckStreamURL = BaseUrl + "/gapi/streaming/check";

        public delegate void OnStreamStatusResponse(bool success, string text, StreamingSessionStatusResponse response);
        
        public static IEnumerator CheckStreamStatus(string sessionToken, string streamToken, OnStreamStatusResponse callback)
        {
            GameStreamSessionModel model = new GameStreamSessionModel();
            model.gameSessionToken = sessionToken;
            model.streamingToken = streamToken;

            yield return SendJsonRequest(CheckStreamURL, "POST", JsonUtility.ToJson(model), (code, json) =>
            {
                StreamingSessionStatusResponse response = JsonUtility.FromJson<StreamingSessionStatusResponse>(json);
                callback(true, string.Empty, response);
            }, (result, er) =>
            {
                callback(false, er, null);
            });
        }

        public delegate void OnRequestStreamResponse(bool success, string text, StreamingCredentialsResponse response);

        public static IEnumerator RequestStream(string sessionToken, bool recording, OnRequestStreamResponse onResponse)
        {
            NewStreamingSessionModel model = new NewStreamingSessionModel();
            model.token = sessionToken;
            model.recording = recording;

            yield return SendJsonRequest(RequestStreamURL, "POST", JsonUtility.ToJson(model), (code, json) =>
            {
                StreamingCredentialsResponse response = JsonUtility.FromJson<StreamingCredentialsResponse>(json);
                onResponse(true, string.Empty, response);
            }, (result, err) =>
            {
                onResponse(false, err, null);
            });
        }

        public static void OpenRegisterPage()
        {
            Application.OpenURL(SignUpUrl);
        }

        public delegate void OnUploadPost(bool success, string text);

        /// <summary>
        /// Uploads a post to API.
        /// User rn can post only every 5 minutes.
        /// </summary>
        /// <param name="sessionToken">Current session token</param>
        /// <param name="image">Image in PNG format</param>
        /// <param name="onUploadPost">Callback when request is done</param>
        /// <returns>Coroutine</returns>
        public static IEnumerator UploadPost(string sessionToken, byte[] image, OnUploadPost onUploadPost)
        {
            WWWForm form = new WWWForm();
            
            form.AddField("sessionToken", sessionToken);
            form.AddBinaryData("image", image);
            
            yield return SendFormRequest(UploadPostUrl, form, (code, resultJson) =>
            {
                GenericResponseModel responseModel = JsonUtility.FromJson<GenericResponseModel>(resultJson);

                if (responseModel != null)
                {
                    onUploadPost(responseModel.successful, responseModel.text);
                }
                else
                {
                    Debug.LogError(resultJson);
                    onUploadPost(false, "Error while parsing result: ");
                }
            }, (result, responseEr) =>
            {
                if (result == UnityWebRequest.Result.ProtocolError)
                {
                    GenericResponseModel responseModel = JsonUtility.FromJson<GenericResponseModel>(responseEr);

                    if (responseModel != null)
                    {
                        onUploadPost(false, responseModel.text);
                    }
                    else
                    {
                        onUploadPost(false, "Error while parsing an error");
                    }
                    return;
                }
                
                onUploadPost(false, "Error while contacting the BRIJ servers");
            });
            
            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionToken"></param>
        /// <param name="postId"></param>
        /// <param name="reaction"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        public static IEnumerator ReactOnPost(string sessionToken, long postId, string reaction, bool remove)
        {
            ReactModel model = new ReactModel();
            model.sessionToken = sessionToken;
            model.postId = postId;
            model.reaction = reaction;
            model.remove = remove;

            yield return SendJsonRequest(ReactUrl, "POST", JsonUtility.ToJson(model), (code, json) =>
            {
                Debug.Log(json);
            }, (result, er) =>
            {
                Debug.Log(er);
            });
            
            yield break;
        }

        public delegate void OnSessionRestore(RestoreSessionModel sessionModel);

        /// <summary>
        /// Restores saved session
        /// </summary>
        /// <param name="sessionToken">Session token</param>
        /// <param name="onSessionRestore">Called when request was handled</param>
        /// <returns>Coroutine</returns>
        public static IEnumerator RestoreSession(string sessionToken, OnSessionRestore onSessionRestore)
        {
            SessionModel model = new SessionModel();
            model.token = sessionToken;

            yield return SendJsonRequest(RestoreSessionUrl, "POST", JsonUtility.ToJson(model), (code, resultJson) =>
            {
                RestoreSessionModel responseModel = JsonUtility.FromJson<RestoreSessionModel>(resultJson);
                onSessionRestore(responseModel);
            }, (result, er) =>
            {
                onSessionRestore(new RestoreSessionModel(false, null));
            });
        }

        public delegate void OnSessionVerify(bool valid);

        /// <summary>
        /// Validates current game session.
        /// If user enters different game they will need to login again.
        /// </summary>
        /// <param name="sessionTokek">Current session token</param>
        /// <param name="onSessionVerify">Called when request is done</param>
        /// <returns></returns>
        public static IEnumerator ValidateSession(string sessionTokek, OnSessionVerify onSessionVerify)
        {
            SessionModel model = new SessionModel();
            model.token = sessionTokek;

            yield return SendJsonRequest(VerifySessionUrl, "POST", JsonUtility.ToJson(model), (code, resultJson) =>
            {
                GenericResponseModel responseModel = JsonUtility.FromJson<GenericResponseModel>(resultJson);
                onSessionVerify(responseModel.successful);
            }, (result, er) =>
            {
                onSessionVerify(false);
            });
        }

        public delegate void OnMetaLoginSuccess(GameSessionModel sessionModel);
        public delegate void OnMetaLoginFailed(string info);

        /// <summary>
        /// Logs in with Meta account, this works only when user is on Meta Quest and game dev correctly set everything
        /// in our API and the game.
        /// </summary>
        /// <param name="oculusNonce">Oculus nonce string</param>
        /// <param name="userId">Meta user id</param>
        /// <param name="gameToken">Game token</param>
        /// <param name="loginSuccess">Called on success</param>
        /// <param name="loginFailed">Called when failed</param>
        /// <returns></returns>
        public static IEnumerator LoginWithMeta(string oculusNonce, ulong userId, string gameToken, OnMetaLoginSuccess loginSuccess, OnMetaLoginFailed loginFailed)
        {
            MetaLoginDataModel model = new MetaLoginDataModel();
            model.oculusNonce = oculusNonce;
            model.userId = userId;
            model.gameToken = gameToken;
            
            yield return SendJsonRequest(LoginWithMetaUrl, "POST", JsonUtility.ToJson(model), (code, resultJson) =>
            {
                GameSessionModel gameSessionModel = JsonUtility.FromJson<GameSessionModel>(resultJson);

                if (!gameSessionModel.successful)
                {
                    loginFailed("Error while processing the request.");
                    return;
                }

                loginSuccess(gameSessionModel);
            }, (result, responseEr) =>
            {
                if (result == UnityWebRequest.Result.ProtocolError)
                {
                    GenericResponseModel responseModel = JsonUtility.FromJson<GenericResponseModel>(responseEr);

                    if (responseModel != null)
                    {
                        loginFailed(responseModel.text);
                    }
                    else
                    {
                        loginFailed("Error while parsing an error");
                    }
                    return;
                }
                
                loginFailed("Error while contacting the BRIJ servers");
            });
            
            yield break;
        }

        public delegate void OnImage(Texture2D texture);
        
        /// <summary>
        /// Gets image from API
        /// </summary>
        /// <param name="image">Image UUID</param>
        /// <param name="onImage">Called when Image request is comple</param>
        /// <returns>Coroutine</returns>
        public static IEnumerator GetImage(string image, OnImage onImage)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(GetImageUrl + image))
            {
                yield return www.SendWebRequest();

                switch (www.result)
                {
                    case UnityWebRequest.Result.InProgress:
                        break;
                    case UnityWebRequest.Result.Success:
                        onImage(DownloadHandlerTexture.GetContent(www));
                        break;
                    case UnityWebRequest.Result.ConnectionError:
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        break;
                }
            }
            
            yield break;
        }
        
        public delegate void OnPostLoaded(List<PostModel> posts);
        public delegate void OnPostLoadError(string text);

        /// <summary>
        /// This method retrieves posts from optional gameId or global post with a range and offset.
        /// </summary>
        /// <param name="session">Current game session</param>
        /// <param name="gameId">Game ID to get all the posts from</param>
        /// <param name="pageNumber">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="postLoadDelegate">Called when posts are loaded</param>
        /// <param name="postLoadErrorDelegate">Called when there was an error loading the posts</param>
        /// <returns>Coroutine</returns>
        public static IEnumerator GetPosts(GameSessionModel session, int gameId, int pageNumber, int pageSize, OnPostLoaded postLoadDelegate, OnPostLoadError postLoadErrorDelegate)
        {
            RequestPostsModel model = new RequestPostsModel();
            model.sessionToken = session.sessionToken;
            model.gameId = gameId;
            model.pageNumber = pageNumber;
            model.pageSize = pageSize;
            
            string json = JsonUtility.ToJson(model);

            yield return SendJsonRequest(GetPostsUrl, "POST", json, (code, resultJson) =>
            {
                PostsResponseModel posts = JsonUtility.FromJson<PostsResponseModel>(resultJson);
                postLoadDelegate(posts.posts);
            }, (result, resultText) =>
            {
                postLoadErrorDelegate(resultText);
            });
            
            yield return null;
        }

        public delegate void OnLoginSuccessCallback(GameSessionModel session);
        public delegate void OnLoginFailedCallback(string reason);
        
        /// <summary>
        /// Tries to login into BRIJ API with supplied email or username and password with an game token.
        /// Single user can be only logged in to only one game and the game is tracked with supplied game token.
        /// If user is loggen with meta, they cannot log in with this until they merge their accounts on BRIJ site.
        /// </summary>
        /// <param name="emailOrUsername">Email or Username</param>
        /// <param name="password">Password</param>
        /// <param name="gameToken">Game token from BRIJ API site</param>
        /// <param name="successCallback">Called when login was successful</param>
        /// <param name="failedCallback">Called when login failed</param>
        /// <returns>Coroutine</returns>
        public static IEnumerator TryLogin(string emailOrUsername, string password, string gameToken, OnLoginSuccessCallback successCallback, OnLoginFailedCallback failedCallback)
        {
            LoginModel model = new LoginModel();
            model.emailOrUsername = emailOrUsername;
            model.password = password;
            model.token = gameToken;

            string json = JsonUtility.ToJson(model);

            yield return SendJsonRequest(NewSessionUrl, "POST", json, (code, resultJson) =>
            {
                // Check for api response code if request was ok
                if (code != 406)
                {
                    GameSessionModel sessionModel = JsonUtility.FromJson<GameSessionModel>(resultJson);

                    if (sessionModel != null)
                    {
                        successCallback(sessionModel);
                    }
                    else
                    {
                        failedCallback("Could not process received data");
                    }
                }
                else
                {
                    GenericResponseModel responseModel = JsonUtility.FromJson<GenericResponseModel>(resultJson);

                    if (responseModel != null)
                    {
                        failedCallback(responseModel.text);
                    }
                    else
                    {
                        failedCallback("Error while parsing an error");
                    }
                }
            }, (result, responseEr) =>
            {
                if (result == UnityWebRequest.Result.ProtocolError)
                {
                    GenericResponseModel responseModel = JsonUtility.FromJson<GenericResponseModel>(responseEr);

                    if (responseModel != null)
                    {
                        failedCallback(responseModel.text);
                    }
                    else
                    {
                        failedCallback("Error while parsing an error");
                    }
                    return;
                }
                
                failedCallback("Error while contacting the BRIJ servers");
            });
        }
        
        public delegate void OnHttpCallbackFailed(UnityWebRequest.Result result, string responseEr);
        public delegate void OnJsonHttpCallback(long responseCode, string json);

        /// <summary>
        /// Send an HTTP request with form data
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="form">Form data</param>
        /// <param name="successCallback">Called on success</param>
        /// <param name="failCallback">Called when failed</param>
        /// <returns>Coroutine</returns>
        private static IEnumerator SendFormRequest(string url, WWWForm form, OnJsonHttpCallback successCallback, OnHttpCallbackFailed failCallback)
        {
            using UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
            
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();
            
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.InProgress:
                    break;
                case UnityWebRequest.Result.Success:
                    string response = webRequest.downloadHandler.text;
                    successCallback(webRequest.responseCode, response);
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    failCallback(UnityWebRequest.Result.ConnectionError, string.Empty);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    failCallback(UnityWebRequest.Result.DataProcessingError, string.Empty);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    string responseEr = webRequest.downloadHandler.text;
                    failCallback(UnityWebRequest.Result.ProtocolError, responseEr);
                    break;
            }
            
            yield break;
        }
        
        /// <summary>
        /// Sends a HTTP request with required JSON input and output.
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="methodType">Method type</param>
        /// <param name="json">Stringified json</param>
        /// <param name="successCallback">Called on success</param>
        /// <param name="failCallback">Called when failed</param>
        /// <returns></returns>
        private static IEnumerator SendJsonRequest(string url, string methodType, string json, OnJsonHttpCallback successCallback, OnHttpCallbackFailed failCallback)
        {
            using UnityWebRequest webRequest = new UnityWebRequest(url, methodType);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            byte[] rawData = Encoding.UTF8.GetBytes(json);

            webRequest.uploadHandler = new UploadHandlerRaw(rawData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.InProgress:
                    break;
                case UnityWebRequest.Result.Success:
                    string response = webRequest.downloadHandler.text;
                    successCallback(webRequest.responseCode, response);
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    failCallback(UnityWebRequest.Result.ConnectionError, string.Empty);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    failCallback(UnityWebRequest.Result.DataProcessingError, string.Empty);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    string responseEr = webRequest.downloadHandler.text;
                    failCallback(UnityWebRequest.Result.ProtocolError, responseEr);
                    break;
            }
            
            yield break;
        }
    }
}