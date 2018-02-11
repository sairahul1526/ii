//#if AADOTWEEN
/***********************************************************************************************************
 * Produced by App Advisory	- http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/


using UnityEngine;
using System.Collections;
//using Vectrosity;

//namespace AppAdvisory.ii
//{
	public class DotManager : MonoBehaviour 
	{

		public bool isOnCircle;

		public SpriteRenderer phareSpriteTOP;
		public SpriteRenderer phareSpriteBOTTOM;

		public SpriteRenderer DotSprite;

		public Transform CircleTop;

		public Transform CircleBottom;

		GameManager gameManager;

		void Awake()
		{
			gameManager = FindObjectOfType<GameManager>();

			Reset ();

		}

		void OnSpawned()
		{


			isOnCircle = false;
			Reset ();
		}

		void OnDespawned()
		{


			StopAllCoroutines ();
			Reset ();
		}


		void Reset()
		{

			if (CircleTop == null)
			{
				CircleTop = GameObject.Find ("CircleTOP").transform;
			}

			if (CircleBottom == null)
			{
				CircleBottom = GameObject.Find ("CircleBOTTOM").transform;
			}

			if (DotSprite == null)
			{
				DotSprite = GetComponent<SpriteRenderer> ();
			}

			DotSprite.color = gameManager.GetDotColor();
			phareSpriteBOTTOM.color = gameManager.GetDotColor();
			phareSpriteTOP.color = gameManager.GetDotColor();

			phareSpriteBOTTOM.gameObject.SetActive (false);


			phareSpriteTOP.gameObject.SetActive (false);

			GetComponent<Collider2D>().enabled = false;

			StopAllCoroutines ();

			if (GetComponent<Rigidbody2D>() == null)
			{
				gameObject.AddComponent<Rigidbody2D> ();
			}

			transform.localScale = Vector3.one;

		}

		public void Replace()
		{

			if (CircleTop == null)
			{
				CircleTop = GameObject.Find ("CircleTOP").transform;
			}


			if (CircleBottom == null)
			{
				CircleBottom = GameObject.Find ("CircleBOTTOM").transform;
			}

			if (DotSprite == null)
			{
				DotSprite = GetComponent<SpriteRenderer> ();
			}

			DotSprite.color = gameManager.GetDotColor();



			phareSpriteBOTTOM.gameObject.SetActive (false);


			phareSpriteTOP.gameObject.SetActive (false);

			GetComponent<Collider2D>().enabled = false;

			StopAllCoroutines ();

			if (GetComponent<Rigidbody2D>() == null)
			{
				gameObject.AddComponent<Rigidbody2D> ();
			}


			transform.localScale = Vector3.one;

		}

		public void ActivateLine(Vector3 target, Transform CircleBorder)
		{
			GetComponent<Collider2D>().isTrigger = false;
			GetComponent<Collider2D>().enabled = true;

			transform.position = target;
			transform.rotation = Quaternion.Euler (0, 0, 0);
			transform.parent = CircleBorder;

			transform.localScale = Vector3.one;

			if (transform.parent.name.Contains("TOP"))
			{

				phareSpriteTOP.gameObject.SetActive (true);
			} else {
				phareSpriteBOTTOM.gameObject.SetActive (true);

			}
		}

		void OnTriggerEnter2D(Collider2D col)
		{

			GameOverLogic (col.gameObject);

		}

		void OnCollisionEnter2D(Collision2D col)
		{

			GameOverLogic (col.gameObject);

		}

		void GameOverLogic(GameObject col)
		{
			if( !gameManager.ISGameOver &&  col.name.Contains("Dot") )
			{

				gameManager.DOGameOver();
			}
		}
	}
//}
//#endif