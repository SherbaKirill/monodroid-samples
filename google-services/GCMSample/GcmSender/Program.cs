﻿using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GcmSender
{
	class MainClass
	{
		public const string API_KEY = "AIzaSyDTcXS-iuXuvznXpXa3tqrW6b67PA_We8Y";

		public static void Main (string[] args)
		{
			if (args.Length < 1 || args.Length > 2 || args [0] == null) {
				Console.Error.WriteLine ("usage: ./gradlew run -Pargs=\"MESSAGE[,DEVICE_TOKEN]\"");
				Console.Error.WriteLine ("");
				Console.Error.WriteLine ("Specify a test message to broadcast via GCM. If a device's GCM registration token is\n" +
				"specified, the message will only be sent to that device. Otherwise, the message \n" +
				"will be sent to all devices subscribed to the \"global\" topic.");
				Console.Error.WriteLine ("");
				Console.Error.WriteLine ("Example (Broadcast):\n" +
				"On Windows:   .\\gradlew.bat run -Pargs=\"<Your_Message>\"\n" +
				"On Linux/Mac: ./gradlew run -Pargs=\"<Your_Message>\"");
				Console.Error.WriteLine ("");
				Console.Error.WriteLine ("Example (Unicast):\n" +
				"On Windows:   .\\gradlew.bat run -Pargs=\"<Your_Message>,<Your_Token>\"\n" +
				"On Linux/Mac: ./gradlew run -Pargs=\"<Your_Message>,<Your_Token>\"");
				Environment.Exit (1);
			}
			try {
				var jGcmData = new JObject ();
				var jData = new JObject ();
				jData.Add ("message", args [0].Trim ());

				if (args.Length > 1 && args [1] != null) {
					jGcmData.Add ("to", args [1].Trim ());
				} else {
					jGcmData.Add ("to", "/topics/global");
				}

				jGcmData.Add ("data", jData);

				var url = new Uri ("https://android.googleapis.com/gcm/send");
				using (var client = new HttpClient ()) {
					var request = new HttpRequestMessage () {
						RequestUri = url,
						Method = HttpMethod.Post
					};
					request.Headers.Accept.Add (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue ("application/json"));
					request.Content = new StringContent (jGcmData.ToString ());
					request.Headers.TryAddWithoutValidation("Authorization", "key=" + API_KEY);
					Task.WaitAll(client.SendAsync (request).ContinueWith (responseTask => {
						var response = responseTask.Result;
						Console.WriteLine (response);
						Console.WriteLine ("Check your device/emulator for notification or logcat for " +
						"confirmation of the receipt of the GCM message.");
					}));
				}

			} catch (IOException e) {
				Console.WriteLine ("Unable to send GCM message.");
				Console.WriteLine ("Please ensure that API_KEY has been replaced by the server " +
				"API key, and that the device's registration token is correct (if specified).");
				Console.Error.WriteLine (e.StackTrace);
			}
		}
	}
}
