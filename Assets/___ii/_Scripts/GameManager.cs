//#if AADOTWEEN
/***********************************************************************************************************
 * Produced by App Advisory	- http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
//#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
//#endif
#if APPADVISORY_ADS
using AppAdvisory.Ads;
#endif
#if VS_SHARE
using AppAdvisory.SharingSystem;
#endif
//using Assets.UI;
#if APPADVISORY_LEADERBOARD
using AppAdvisory.social;
#endif

//namespace AppAdvisory.ii
//{
	public class GameManager : MonoBehaviour {

		public int numberOfPlayToShowInterstitial = 5;

		public string VerySimpleAdsURL = "http://u3d.as/oWD";

		Text textPreviousLevel;
		Text textNextLevel;
		public Text levelText;

		public Button buttonPrevious;
		public Button buttonNext;

		List<DotManager> DotsBottom;
		List<DotManager> DotsTop;

		Vector3 rotateVectorTOP;
		Vector3 rotateVectorBOTTOM;

		private Ease easeType = Ease.Linear;

		private LoopType loopType = LoopType.Yoyo;

		private float rotateCircleDelay = 6f;

		private int numberDotsToCreate;

		private int numberDotsOnCircle;


		private float ratioNombreDotsOnCircleTop;
		private float ratioNombreDotsOnCircleBottom;

		private float rationNombreDotsToCreatTop;
		private float rationNombreDotsToCreatBottom;


		public float ratioRotationSpeedTOP;
		public float ratioRotationSpeedBOTTOM;


		float centerCircle;

		AudioSource audioSource;
		public AudioClip MUSIC;
		public AudioClip FX_FAIL;
		public AudioClip FX_SUCCESS;
		public AudioClip FX_SHOOT;
		void PlaySoundFail()
		{
			audioSource.PlayOneShot(FX_FAIL);
		}
		void PlaySoundSuccess()
		{
			audioSource.PlayOneShot(FX_SUCCESS);
		}
		void PlaySoundShoot()
		{
			audioSource.PlayOneShot(FX_SHOOT);
		}


		SpriteRenderer[] allSprites
		{
			get
			{
				return FindObjectsOfType<SpriteRenderer>();
			}
		}

		public Color GetDotColor()
		{
			return this.DotColor;
		}

		public bool ISGameOver
		{
			get
			{
				return this.isGameOver;
			}
		}

		public Color FailColor;

		public Color SuccessColor;

		public Color BackgroundColor;

		public Color DotColor;

		public int Level;

		[NonSerialized]
		public bool success;
		public bool isGameOver;
		public string poolName;

		Camera cam;
		float height;

		public Transform CircleTOP;
		public Transform CircleBorderTOP;
		float positionTouchBorderTOP;

		public Transform CircleBOTTOM;
		public Transform CircleBorderBOTTOM;
		float positionTouchBorderBOTTOM;

		public SpriteRenderer CircleCenterSprite;
		public SpriteRenderer CircleCenterSprite2;

		public GameObject DotPrefab;

		public float speed = 1f;

		float sizeBorder;

		public Transform CircleSprite;

		void Awake()
		{
			audioSource = GetComponent<AudioSource>();

			cam = Camera.main;
			height = 2f * cam.orthographicSize;

			isGameOver = true;

			CircleTOP.position = new Vector3 (0, height / 3f, 0);
			CircleBOTTOM.position = new Vector3 (0, -height / 3f, 0);

			cam.transform.position = new Vector3 (0, 0, -10);

			CreateGame ();

			foreach(SpriteRenderer s in allSprites)
			{
				s.color = BackgroundColor;
			}

			DOTween.Init();
		}

		void CleanMemory()
		{
			GC.Collect();
			Application.targetFrameRate = 60;
			Time.fixedDeltaTime = 1f/60f;
			Time.maximumDeltaTime = 3 * Time.fixedDeltaTime;
			GC.Collect();
		}

		int levelPlayed
		{
			get
			{
				int lp = PlayerPrefs.GetInt ("LEVEL_PLAYED", 1);

				if(lp == 0)
				{
					lp = 1;
					levelPlayed = 1;
				}


				return lp;
			}
			set
			{
				int lp = value;
				if(lp <= 0)
					lp = 1;

				PlayerPrefs.SetInt ("LEVEL_PLAYED", lp);
				PlayerPrefs.Save();
			}
		}

		int level
		{
			get
			{
				int lp = PlayerPrefs.GetInt ("LEVEL", 1);

				if(lp == 0)
				{
					lp = 1;
					level = 1;
				}

				return lp;
			}
			set
			{
				int lp = value;
				if(lp <= 0)
					lp = 1;

				PlayerPrefs.SetInt ("LEVEL", lp);
				PlayerPrefs.Save();
			}
		}

		void SetLevelButtonsVisibility()
		{
//			if(levelPlayed < level)
//				buttonNext.gameObject.SetActive(true);
//			else
//				buttonNext.gameObject.SetActive(false);
//
//			if(levelPlayed > 1)
//				buttonPrevious.gameObject.SetActive(true);
//			else
//				buttonPrevious.gameObject.SetActive(false);
		}

		public void OnClickedButtonLast()
		{
			int levelPlayerNew = Mathf.Max(0,levelPlayed - 1);

			print("levelPlayerNew = " + levelPlayerNew);

			levelPlayed = levelPlayerNew;

			CleanMemory();

			SetTextLevel();
		}

		public void OnClickedButtonNext()
		{

			int levelPlayerNew = Mathf.Min(level, levelPlayed + 1);

			levelPlayed = levelPlayerNew;

			CleanMemory();

			SetTextLevel();
		}

		void RestartLevel(bool success)
		{
			if (success)
			{
				levelPlayed++;

				int max = Mathf.Max (level, levelPlayed);

				level = max;

				#if APPADVISORY_LEADERBOARD
				LeaderboardManager.ReportScore(level);
				#endif
			}

			CleanMemory();

			#if UNITY_5_3_OR_NEWER
			SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex,LoadSceneMode.Single);
			#else
			Application.LoadLevel(Application.loadedLevel);
			#endif

			CleanMemory();
		}

		public void DOGameOver()
		{
			this.GameOver();
		}

		void LevelCleared()
		{
			ShowAds();

			#if VS_SHARE
			VSSHARE.DOTakeScreenShot();
			#endif

			PlaySoundSuccess();
			isGameOver = true;

			canShoot = false;


			DOColorSuccess(() => {
				RestartLevel (true);
			});
		}

		void GameOver()
		{
			ShowAds();

			#if VS_SHARE
			VSSHARE.DOTakeScreenShot();
			#endif

			DOTween.KillAll();

			StopAllCoroutines ();

			PlaySoundFail();

			isGameOver = true;

			canShoot = false;

			DOColorFail(() => {
				RestartLevel (false);
			});
		}

		void DOColorFail(Action callback)
		{
			canShoot = false;

			foreach(var s in allSprites)
			{
				s.DOColor(FailColor,0.1f).SetEase(Ease.Linear).SetDelay(1f/30f);;
			}

			DOVirtual.DelayedCall(0.3f, () => {
				foreach(var s in allSprites)
				{
					s.DOColor(BackgroundColor,0.3f).SetEase(Ease.Linear);
				}

				DOVirtual.DelayedCall(0.40f, () => {
					if(callback != null)
						callback();
				});
			});
		}

		void DOColorSuccess(Action callback)
		{
			canShoot = false;

			foreach(var s in allSprites)
			{
				s.DOColor(SuccessColor,0.1f).SetEase(Ease.Linear);
			}

			DOVirtual.DelayedCall(0.3f, () => {
				foreach(var s in allSprites)
				{
					s.DOColor(BackgroundColor,0.3f).SetEase(Ease.Linear);
				}

				DOVirtual.DelayedCall(0.40f, () => {
					if(callback != null)
						callback();
				});
			});
		}

		private void ContinueGame()
		{
			StartCoroutine (SetBoolWithDelay (1));
		}

		IEnumerator SetBoolWithDelay(float delay)
		{
			yield return new WaitForSeconds (delay);

			StopCoroutine ("ShootDot");

			foreach (Transform t in CircleBorderTOP)
			{
				t.GetComponent<DotManager>().DotSprite.color = this.DotColor;
			}

			foreach (Transform t in CircleBorderBOTTOM)
			{
				t.GetComponent<DotManager>().DotSprite.color = this.DotColor;
			}

			if (lastShootTop != null)
			{
				lastShootTop.Replace ();
				lastShootTop.transform.SetParent (transform);
				DotsTop.Add (lastShootTop);
			}
			if (lastShootBottom != null)
			{
				lastShootBottom.Replace ();
				lastShootBottom.transform.SetParent (transform);
				DotsBottom.Add (lastShootBottom);
			}

			StartCoroutine (PositioningDots ());

			yield return new WaitForSeconds (delay);

			LaunchRotateCircle ();

			isGameOver = false;
			canShoot = true;
		}


		public void OnclickedPlay ()
		{
			Application.targetFrameRate = 60;
			GC.Collect();

			success = false;
			canShoot = true;
			isGameOver = false;

			LaunchRotateCircle ();

			foreach(SpriteRenderer s in allSprites)
			{
				s.color = BackgroundColor;
				s.DOColor(DotColor,0.3f).SetEase(Ease.Linear);
			}

			#if VS_SHARE
			VSSHARE.DOHideScreenshotIcon();
			#endif

		}



		void SetTextLevel()
		{
			textPreviousLevel = FindObjectOfType<UIController>().textLast;
			textNextLevel = FindObjectOfType<UIController>().textBest;

			textPreviousLevel.gameObject.SetActive(false);
			textNextLevel.text = "LEVEL\n" + levelPlayed;

			levelText.text = "LEVEL " + level;

			SetLevelButtonsVisibility();
		}

		public void CreateGame()
		{
			print("***** CreateGame *****");
			SetTextLevel();

			transformVectorLines = new List<Transform> ();

			canShoot = false;

			cam.orthographicSize = 20f;
			cam.transform.position = new Vector3 (0, 0, -10);

			StopAllCoroutines ();

			isGameOver = true;
			cam.backgroundColor = this.BackgroundColor; //this.GRAY;
			CircleCenterSprite.color = this.DotColor;
			CircleCenterSprite2.color = this.DotColor;

			Level l = LevelManager.GetLevel (levelPlayed);

			numberDotsOnCircle = l.numberDotsOnCircle;
			numberDotsToCreate = l.numberDotsToCreate;

			rotateCircleDelay = l.rotateDelay;
			easeType = l.rotateEaseType;
			loopType = l.rotateLoopType;


			ratioNombreDotsOnCircleTop = l.ratioNombreDotsOnCircleTop;
			ratioNombreDotsOnCircleBottom = l.ratioNombreDotsOnCircleBottom;
			rationNombreDotsToCreatTop = l.rationNombreDotsToCreatTop;
			rationNombreDotsToCreatBottom = l.rationNombreDotsToCreatBottom;

			ratioRotationSpeedTOP = l.ratioRotationSpeedTOP;
			ratioRotationSpeedBOTTOM = l.ratioRotationSpeedBOTTOM;


			centerCircle = height / 4.5f;

			CircleTOP.position = new Vector3 (0, centerCircle, 0);
			CircleBOTTOM.position = new Vector3 (0, -centerCircle, 0);

			positionTouchBorderTOP = centerCircle - CircleTOP.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.y;

			positionTouchBorderBOTTOM = -positionTouchBorderTOP;


			rotateVectorTOP = new Vector3 (0, 0, 1);
			if (Level % 2 == 0)
			{
				rotateVectorTOP = new Vector3 (0, 0, -1);
			}

			if (Level % 3 == 0)
			{
				rotateVectorBOTTOM = -rotateVectorTOP;
			} else {
				rotateVectorBOTTOM = rotateVectorTOP;
			}



			CircleTOP.rotation = Quaternion.Euler (Vector3.zero);
			CircleBOTTOM.rotation = Quaternion.Euler (Vector3.zero);

			CircleBorderTOP.rotation = Quaternion.Euler (Vector3.zero);
			CircleBorderBOTTOM.rotation = Quaternion.Euler (Vector3.zero);




			CircleBorderTOP.localScale = Vector3.one;
			CircleBorderBOTTOM.localScale = Vector3.one;



			levelText.text = "LEVEL " + levelPlayed;


			CreateDotOnCircle ();

			CreateListDots ();


			StartCoroutine (PositioningDots ());


			PositioningCamera ();

		}

		void PositioningCamera()
		{
			float H = Screen.height;
			float sizeBanner = 50f;

			if (PlayerPrefs.HasKey ("NOADS"))
			{
				if (PlayerPrefs.GetInt ("NOADS") == 1)
				{
					sizeBanner = 0f;
				}
			} 

			float ratio = (H - sizeBanner) / H;

			float positionCamY = (1f - ratio) * this.height;

			cam.transform.position = new Vector3 (0, -positionCamY, cam.transform.position.z);
		}

		public List<Transform> transformVectorLines;

		void LaunchRotateCircle()
		{
			CircleTOP.rotation = Quaternion.Euler (Vector3.zero);
			CircleBOTTOM.rotation = Quaternion.Euler (Vector3.zero);

			CircleBorderTOP.DORotate (rotateVectorTOP * 360, rotateCircleDelay*ratioRotationSpeedTOP, RotateMode.FastBeyond360)
				.SetEase(easeType)
				.SetLoops(-1, loopType);
			CircleBorderBOTTOM.DORotate (rotateVectorBOTTOM * 360, rotateCircleDelay*ratioRotationSpeedBOTTOM, RotateMode.FastBeyond360)
				.SetEase(easeType)
				.SetLoops(-1, loopType);

		}

		DotManager InstantiateDot(Vector3 posTOP, Quaternion rot, Transform parent)
		{
			var dm = InstantiateDot();

			dm.transform.SetParent(parent);
			dm.transform.position = posTOP;
			dm.transform.rotation = rot;

			return dm;
		}


		void CreateDotOnCircle()
		{

			Time.timeScale = 1;


			GameObject prefab = this.DotPrefab;
			Quaternion rot = prefab.transform.rotation;

			Vector3 posTOP = new Vector3 (0, positionTouchBorderTOP, 0);
			Transform parentTOP = CircleBorderTOP;


			float numberTOP = numberDotsOnCircle * ratioNombreDotsOnCircleTop;


			for (int i = 0; i < (int)numberTOP ; i++)
			{

				CircleTOP.rotation = Quaternion.Euler( new Vector3 (0, 0, i * 360f / (int)numberTOP) );

				DotManager dm = InstantiateDot(posTOP, rot, parentTOP);

				dm.ActivateLine (dm.transform.position,dm.transform.parent);
			}

			Vector3 posBOTTOM = new Vector3 (0, positionTouchBorderBOTTOM, 0);

			float numberBOTTOM = numberDotsOnCircle * ratioNombreDotsOnCircleBottom;

			for (int i = 0; i < (int)numberBOTTOM; i++)
			{

				CircleBOTTOM.rotation = Quaternion.Euler( new Vector3 (0, 0, i * 360f / (int)numberBOTTOM) );

				DotManager dm = InstantiateDot (posBOTTOM, rot, CircleBorderBOTTOM);

				dm.ActivateLine (dm.transform.position,dm.transform.parent);
			}
		}

		private Vector3 GetDotPosition(int i)
		{
			Vector3 target = new Vector3 (i * sizeDot *  1.2f, sizeDot*0.6f, 0);

			return target;
		}

		Vector3 getDotPositionTop(int i)
		{
			return GetDotPosition (i);
		}

		Vector3 getDotPositionBottom(int i)
		{
			return -GetDotPosition (i);
		}

		DotManager InstantiateDot()
		{
			var go = Instantiate(this.DotPrefab) as GameObject;
			return go.GetComponent<DotManager> ();
		}

		void CreateListDots()
		{
			canShoot = false;
			DotsBottom = new List<DotManager>();
			DotsTop = new List<DotManager>();

			if (sizeDot == 0)
			{
				DotManager dm = InstantiateDot();


				sizeDot = dm.DotSprite.bounds.size.x;
			}

			float numberBOTTOM = numberDotsToCreate * rationNombreDotsToCreatBottom;

			for (int i = 0; i < numberBOTTOM ; i++)
			{

				DotManager dm = InstantiateDot();
				dm.GetComponent<Collider2D>().enabled = false;

				dm.transform.position = getDotPositionBottom(i);


				DotsBottom.Add (dm);

			}

			float numberTOP = numberDotsToCreate * rationNombreDotsToCreatTop;

			for (int i = 0; i <numberTOP; i++)
			{



				DotManager dm = InstantiateDot();
				dm.GetComponent<Collider2D>().enabled = false;



				dm.transform.position = getDotPositionTop(i);


				DotsTop.Add (dm);

			}

		}


		private void OnColorUpdated(Color color)
		{
			cam.backgroundColor = color;
		}



		private void OnCamSizeUpdated(float size)
		{
			cam.orthographicSize = size;
		}


		IEnumerator OnAnimationGameToMenuCompleted(float wait)
		{
			yield return new WaitForSeconds (wait);



			CreateGame ();
			isGameOver = true;

			ShowAds ();
		}



		/// <summary>
		/// If using Very Simple Ads by App Advisory, show an interstitial if number of play > numberOfPlayToShowInterstitial: http://u3d.as/oWD
		/// </summary>
		public void ShowAds()
		{
			int count = PlayerPrefs.GetInt("GAMEOVER_COUNT",0);
			count++;
			PlayerPrefs.SetInt("GAMEOVER_COUNT",count);
			PlayerPrefs.Save();

			#if APPADVISORY_ADS
			if(count > numberOfPlayToShowInterstitial)
			{
			#if UNITY_EDITOR
				print("count = " + count + " > numberOfPlayToShowINterstitial = " + numberOfPlayToShowInterstitial);
			#endif
				if(AdsManager.instance.IsReadyInterstitial())
				{
			#if UNITY_EDITOR
					print("AdsManager.instance.IsReadyInterstitial() == true ----> SO ====> set count = 0 AND show interstial");
			#endif
					if(AdsManager.instance.IsReadyInterstitial())
					{
						AdsManager.instance.ShowInterstitial();
						PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
					}
				}
				else
				{
			#if UNITY_EDITOR
					print("AdsManager.instance.IsReadyInterstitial() == false");
			#endif
				}

			}
			else
			{
				PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
			}
			PlayerPrefs.Save();
			#else
			if(count >= numberOfPlayToShowInterstitial)
			{
			Debug.LogWarning("To show ads, please have a look to Very Simple Ad on the Asset Store, or go to this link: " + VerySimpleAdsURL);
			Debug.LogWarning("Very Simple Ad is already implemented in this asset");
			Debug.LogWarning("Just import the package and you are ready to use it and monetize your game!");
			Debug.LogWarning("Very Simple Ad : " + VerySimpleAdsURL);
			PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
			}
			else
			{
			PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
			}
			PlayerPrefs.Save();
			#endif
		}

		void Update ()
		{

	if (Input.GetButtonDown ("Submit") && !isGameOver && canShoot)
			{
				if (DotsTop.Count > 0 || DotsBottom.Count > 0 )
				{
					DotManager dTOP = null;
					DotManager dBOTTOM = null;

					if (DotsTop.Count > 0)
					{
						dTOP = DotsTop [0];
					}

					if (DotsBottom.Count > 0)
					{
						dBOTTOM = DotsBottom [0];
					}

					StartCoroutine (ShootDot (dBOTTOM,dTOP));
//				OnclickedPlay ();
				}
			}
		}



		bool canShoot;

		DotManager lastShootTop;
		DotManager lastShootBottom;

		IEnumerator ShootDot(DotManager dBottom, DotManager dTop)
		{
			canShoot = false;

			lastShootTop = null;
			lastShootBottom = null;

			StopCoroutine("PositioningDots");
			StopCoroutine("MoveStartPositionDot");

			if (dBottom != null)
			{
				dBottom.GetComponent<Collider2D>().enabled = true;
				dBottom.GetComponent<Collider2D>().isTrigger = true;
			}
			if (dTop != null)
			{
				dTop.GetComponent<Collider2D>().enabled = true;
				dTop.GetComponent<Collider2D>().isTrigger = true;
			}

			for (int i = 0; i < DotsTop.Count; i++)
			{



				DotsTop [i].transform.localScale = Vector3.one;


				DotsTop [i].transform.position = getDotPositionTop(i);

			}

			for (int i = 0; i < DotsBottom.Count; i++)
			{


				DotsBottom [i].transform.localScale = Vector3.one;

				DotsBottom [i].transform.position = getDotPositionBottom(i);

			}

			yield return null;


			if (DotsBottom.Count != 0 || DotsTop.Count != 0)
			{

				PlaySoundShoot();

				if (dBottom != null)
				{
					DotsBottom.Remove (dBottom);
					lastShootBottom = dBottom;
				}

				if (dTop != null)
				{
					DotsTop.Remove (dTop);
					lastShootTop = dTop;
				}

				StartCoroutine (PositioningDots ());





				Vector3 targetBttom = new Vector3 (0, positionTouchBorderBOTTOM, 0);
				Vector3 targetTop = new Vector3 (0, positionTouchBorderTOP, 0);




				if (dBottom != null)
				{
					dBottom.transform.position = getDotPositionBottom (0);
					//new Vector3 (0, -sizeDot*0.7f, 0);
				}

				if (dTop != null)
				{
					dTop.transform.position = getDotPositionTop (0);
					//new Vector3 (0, sizeDot*0.7f, 0);
				}

				while (true)
				{
					float step = speed * Time.deltaTime;
					if (dBottom != null)
					{
						dBottom.transform.position = Vector3.MoveTowards (dBottom.transform.position, targetBttom, step);
					}

					if (dTop != null)
					{
						dTop.transform.position = Vector3.MoveTowards (dTop.transform.position, targetTop, step);
					}

					if (dBottom != null)
					{
						if (dBottom.transform.position == targetBttom || isGameOver)
						{

							canShoot = true;


							break;
						}
					} else {

						if (dTop.transform.position == targetTop || isGameOver)
						{

							canShoot = true;


							break;
						}

					}


					yield return new WaitForEndOfFrame ();
				}

				if (dBottom != null)
				{
					dBottom.ActivateLine (targetBttom, CircleBorderBOTTOM);
				}

				if (dTop != null)
				{
					dTop.ActivateLine (targetTop, CircleBorderTOP);
				}

				yield return new WaitForSeconds (0.001f);
				if (DotsBottom.Count == 0 && DotsTop.Count == 0 && !isGameOver)
				{

					success = true;
				}

				if (success && !isGameOver) 
				{
					LevelCleared();
				}

				for (int i = 0; i < DotsTop.Count; i++)
				{

					DotsTop [i].transform.localScale = Vector3.one;

					DotsTop [i].transform.position = getDotPositionTop(i);

				}

				for (int i = 0; i < DotsBottom.Count; i++)
				{

					DotsBottom [i].transform.localScale = Vector3.one;

					DotsBottom [i].transform.position = getDotPositionBottom(i);
				}
			}
		}

		float sizeDot = 0;
		IEnumerator PositioningDots()
		{
			for (int i = 0; i < DotsTop.Count; i++)
			{
				if (DotsTop.Count > 0)
				{
					DotsTop [i].transform.localScale = Vector3.one;

					DotsTop [i].transform.DOMove (getDotPositionTop(i), speed / 500f).SetEase(Ease.Linear);

					DotsTop [i].GetComponent<Collider2D>().enabled = false;
				}
			}

			for (int i = 0; i < DotsBottom.Count; i++)
			{
				if (DotsBottom.Count > 0)
				{
					DotsBottom [i].transform.localScale = Vector3.one;

					DotsBottom [i].transform.DOMove (getDotPositionBottom(i), speed / 500f).SetEase(Ease.Linear);

					DotsBottom [i].GetComponent<Collider2D>().enabled = false;
				}

				yield return null;
			}
		}

		public void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				//Debug.Log ("OnApplicationPause FALSE");
				Resources.UnloadUnusedAssets ();
				Time.timeScale = 1.0f;
			} else {
				//Debug.Log ("OnApplicationPause TRUE");
				Resources.UnloadUnusedAssets ();
				Time.timeScale = 0.0f;
			}
		}  

		void OnApplicationQuit()
		{
			PlayerPrefs.Save();
		}

		void OnDetectAppUpdated()
		{
			//Debug.Log("A new version is installed. Current version: " + //UniRate.Instance.applicationVersion);
			PlayerPrefs.SetInt ("PLAY_COUNT",0);
		}

		void OnUserAttemptToRate()
		{
			//Debug.Log("Yeh, great, user want to rate us!");
			PlayerPrefs.SetInt ("PLAY_COUNT",-1000);
		}

		void OnUserDeclinedToRate()
		{
			//Debug.Log("User declined the rate prompt.");
			PlayerPrefs.SetInt ("PLAY_COUNT",-1000);
		}

		void OnUserWantReminderToRate()
		{
			//Debug.Log("User wants to be reminded later.");
			PlayerPrefs.SetInt ("PLAY_COUNT",-15);
		}
	}
//}
//#endif