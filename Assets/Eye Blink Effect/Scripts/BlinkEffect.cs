using UnityEngine;
using System.Collections;

namespace PostProcess
{
	[ExecuteInEditMode]
	[RequireComponent (typeof (Camera))]
	[AddComponentMenu ("Image Effects/Blink Effect")]
	public class BlinkEffect : MonoBehaviour
	{
		public Shader standard;
		public Shader curved;

		enum State
		{
			FadingIn,
			FadingOut,
			WaitingForFadeOut,
			Idle
		}

		[Range (0f, 1f)]
		public float smoothness = 0.96f;

		[Range (0f, 1f)]
		public float curvature = 1.0f;

		[Range (0f, 1f)]
		public float time = 0.0f;

		[Range (0f, 10f)]
		public float fadeOutDelay = 0.0f;

		float fadeInTime = 1f;
		float fadeOutTime = 1f;

		public AnimationCurve fadeInCurve;
		public AnimationCurve fadeOutCurve;

		Material material;
		Material materialCurved;

		float localTime;
		State state;
		bool inAndOut;
		
		System.Action onFadeInComplete;
		System.Action onFadeOutComplete;

		void Awake () 
		{
			standard = Shader.Find ("Hidden/Image Effects/Blink");
			curved = Shader.Find ("Hidden/Image Effects/Blink Curved");

			SetDefaultFadeInAnimationCurves ();
			SetDefaultFadeOutAnimationCurves ();
			time = 0f;
			localTime = 0f;
			state = State.Idle;
			inAndOut = true;
			material = new Material (standard);
			materialCurved = new Material (curved);
		}

		void OnRenderImage (RenderTexture source, RenderTexture destination)
		{
			Material preferredMaterial = materialCurved;

			if (Mathf.Approximately (curvature, 0f))
				preferredMaterial = material;
			else 
				preferredMaterial.SetFloat ("_Curvature", curvature * 0.297f);

			float smooth = 80f - smoothness * 75.2f;
			preferredMaterial.SetFloat ("_LocalTime", time - 0.3f);
			preferredMaterial.SetFloat ("_Smoothness", smooth);
			Graphics.Blit (source, destination, preferredMaterial);
		}

		#if UNITY_EDITOR
		float prevLocalTime;
		Camera cameraRef;
		
		public void RunEditorPreview ()
		{
			prevLocalTime = Time.realtimeSinceStartup;
			Blink (null, null);
			UnityEditor.EditorApplication.update -= OnEditorUpdate;
			UnityEditor.EditorApplication.update += OnEditorUpdate;
			cameraRef = GetComponent<Camera> ();
		}
		
		void OnEditorUpdate () 
		{
			Update ();
			cameraRef.Render ();
		}
		#endif

		void SetDefaultFadeInAnimationCurves () 
		{
			Keyframe begin = new Keyframe ();
			begin.time = 0f;
			begin.value = 0f;

            Keyframe key1 = new Keyframe();
            key1.time = 1.5f;
            key1.value = 0.698f;

            Keyframe key2 = new Keyframe();
            key2.time = 2.5f;
            key2.value = 0.3f;

            Keyframe key3 = new Keyframe();
            key3.time = 4f;
            key3.value = 0.8f;

            Keyframe key4 = new Keyframe();
            key4.time = 5.0f;
            key4.value = 0.327f;

            Keyframe end = new Keyframe ();
			end.time = 6.5f;
			end.value = 1f;
			
			fadeInCurve = new AnimationCurve ();
			fadeInCurve.AddKey (begin);
            fadeInCurve.AddKey(key1);
            fadeInCurve.AddKey(key2);
            fadeInCurve.AddKey(key3);
            fadeInCurve.AddKey(key4);
            fadeInCurve.AddKey (end);
			fadeInCurve.postWrapMode = WrapMode.Clamp;
			fadeInCurve.preWrapMode = WrapMode.Clamp;
		}
		
		void SetDefaultFadeOutAnimationCurves () 
		{
			Keyframe begin = new Keyframe ();
			begin.time = 0f;
			begin.value = 1f;
			
			Keyframe end = new Keyframe ();
			end.time = 1f;
			end.value = 1f;
			
			fadeOutCurve = new AnimationCurve ();
			fadeOutCurve.AddKey (begin);
			fadeOutCurve.AddKey (end);
			fadeOutCurve.postWrapMode = WrapMode.Clamp;
			fadeOutCurve.preWrapMode = WrapMode.Clamp;
		}
	
		void Update ()
		{
			if (state == State.Idle)
				return;
			
			#if UNITY_EDITOR
			if (Application.isPlaying)
				localTime += Time.deltaTime;
			else {
				float now = Time.realtimeSinceStartup;
				localTime += now - prevLocalTime;
				prevLocalTime = now;
			}
			#else
			localTime += Time.deltaTime;
			
			#endif
			
			if (state == State.FadingIn) {
				time = fadeInCurve.Evaluate (localTime);
				
				if (localTime > fadeInTime) {
					time = 1f;
					localTime = 0f;
					if (inAndOut) {
						if (fadeOutDelay == 0f) {
							state = State.FadingOut;
						} else
							state = State.WaitingForFadeOut;
					} else {
						state = State.Idle;
					}
					
					if (onFadeInComplete != null) {
						onFadeInComplete ();
						onFadeInComplete = null;
					}
				}
			} else if (state == State.FadingOut) {
				time = fadeOutCurve.Evaluate (localTime);
				
				if (localTime > fadeOutTime) {
					time = 0f;
					localTime = 0f;
					state = State.Idle;
					
					#if UNITY_EDITOR
					UnityEditor.EditorApplication.update -= OnEditorUpdate;
					#endif
					
					if (onFadeOutComplete != null) {
						onFadeOutComplete ();
						onFadeOutComplete = null;
					}
				}
			} else if (state == State.WaitingForFadeOut) {
				if (localTime > fadeOutDelay) {
					localTime = 0f;
					state = State.FadingOut;
				}
			}
		}

		public void Blink (System.Action onComplete = null, System.Action onFadeInComplete = null)
		{
			inAndOut = true;
			this.onFadeOutComplete = onComplete;
			this.onFadeInComplete = onFadeInComplete;
			time = 0f;
			localTime = 0f;
			fadeInTime = fadeInCurve [fadeInCurve.length - 1].time;
			fadeOutTime = fadeOutCurve [fadeOutCurve.length - 1].time;
			state = State.FadingIn;
		}
		
		public void FadeIn (System.Action onComplete = null)
		{
			this.onFadeInComplete = onComplete;
			this.onFadeOutComplete = null;
			state = State.FadingIn;
			inAndOut = false;
			fadeInTime = fadeInCurve [fadeInCurve.length - 1].time;
			time = 0f;
			localTime = 0f;
		}
		
		public void FadeOut (System.Action onComplete = null)
		{
			this.onFadeInComplete = null;
			this.onFadeOutComplete = onComplete;
			state = State.FadingOut;
			inAndOut = false;
			fadeOutTime = fadeOutCurve [fadeOutCurve.length - 1].time;
			time = 1f;
			localTime = 0f;
		}
	}
}