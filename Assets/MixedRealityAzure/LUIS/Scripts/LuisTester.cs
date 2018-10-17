﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Microsoft.Cognitive.LUIS;
using Microsoft.MR.LUIS;
using Microsoft.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides a quick way to test LUIS in a Unity scene.
/// </summary>
public class LuisTester : MonoBehaviour
{
	#region Unity Inspector Variables
	[Tooltip("Enable debug handlers to increase debug information.")]
	public bool EnableDebugging = true;

	[Tooltip("The LuisManager that the tester will interface with.")]
	public LuisManager LuisManager;

	[Tooltip("Predict the Test Utterance automatically on start.")]
	public bool PredictOnStart = true;

    [Tooltip("Optional UI Button in the scene that can initiate the prediction.")]
    public Button SceneTestButton;

    [Tooltip("Optional UI Input field in the scene that supplies the test utterance.")]
    public InputField SceneUtteranceInput;

	[Tooltip("The utterance to test")]
	public string TestUtterance = "";
	#endregion // Unity Inspector Variables

	#region Unity Overrides
	void Start()
	{
		// If no manager specified, see if one is on the same GameObject
		if (LuisManager == null)
		{
			LuisManager = GetComponent<LuisManager>();
		}

		// Validate components
		if (LuisManager == null)
		{
			Debug.LogErrorFormat("The {0} inspector field is not set and is required. {1} did not load.", nameof(LuisManager), this.GetType().Name);
			enabled = false;
			return;
		}

        // If there is a test button in the scene, wire up the click handler.
        if (SceneTestButton != null)
        {
            SceneTestButton.onClick.AddListener(() =>
            {
                TryPredict();
            });
        }

        // If there is a test text field, setup the default
        if ((SceneUtteranceInput != null) && (string.IsNullOrEmpty(SceneUtteranceInput.text)))
        {
            SceneUtteranceInput.text = TestUtterance;
        }

		// Enable debugging?
		if (EnableDebugging)
		{
			LuisManager.IntentHandlers.Add(new DebugIntentHandler());
		}

		// Predict on start?
		if ((PredictOnStart) && (!string.IsNullOrEmpty(TestUtterance)))
		{
			TryPredict();
		}
	}
	#endregion // Unity Overrides

	#region Public Methods
	/// <summary>
	/// Attempts to try a prediction of the <see cref="TestUtterance"/>.
	/// </summary>
	public async void TryPredict()
	{
        // Make sure we're enabled
		if (!enabled)
		{
			Debug.LogError($"{nameof(LuisTester)} is not enabled. Can't predict.");
			return;
		}

        // Make sure we have a Luis Manager assigned
		if (LuisManager == null)
		{
			Debug.LogError($"{nameof(LuisManager)} is not set to a valid instance.");
			return;
		}

        // If there is a scene text control and its contents aren't empty use that
        if ((SceneUtteranceInput != null) && (!string.IsNullOrEmpty(SceneUtteranceInput.text)))
        {
            TestUtterance = SceneUtteranceInput.text;
        }

        // Make sure we have something to predict
		if (string.IsNullOrEmpty(TestUtterance))
		{
			Debug.LogError($"{nameof(TestUtterance)} is empty. Nothing to predict.");
			return;
		}

		// Predict!
		await LuisManager.PredictAndHandleAsync(TestUtterance);
	}
	#endregion // Public Methods
}
