using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * Class with a collection of generic functionalities
	 * 
	 * @author Esteban Gallardo
	 */
	public class Utilities
	{
		// CONSTANTS DIRECTIONS
		public const int DIRECTION_LEFT = 1;
		public const int DIRECTION_RIGHT = 2;
		public const int DIRECTION_UP = 100;
		public const int DIRECTION_DOWN = 200;
		public const int DIRECTION_NONE = -1;

		public static string[] IgnoreLayersForDebug = new string[] { "UI" };

		// -------------------------------------------
		/* 
		 * Get distance between points in axis X,Z
		 */
		public static float DistanceXZ(Vector3 _positionA, Vector3 _positionB)
		{
			return Mathf.Abs(_positionA.x - _positionB.x) + Mathf.Abs(_positionA.z - _positionB.z);
		}

		// -------------------------------------------
		/* 
		 * Get distance between points in axis X,Z
		 */
		public static float DistanceSqrtXZ(Vector3 _positionA, Vector3 _positionB)
		{
			float x = Mathf.Abs(_positionA.x - _positionB.x);
			float y = Mathf.Abs(_positionA.z - _positionB.z);
			return Mathf.Sqrt((x * x) + (y * y));
		}

		// -------------------------------------------
		/* 
		 * Adds a child to the parent
		 */
		public static GameObject AddChild(Transform _parent, GameObject _prefab)
		{
			GameObject newObj = GameObject.Instantiate(_prefab);
			newObj.transform.SetParent(_parent, false);
			return newObj;
		}

		// -------------------------------------------
		/* 
		 * Adds a sprite component to the object.
		 * It's used to create the visual selectors.
		 */
		public static Sprite AddSprite(GameObject _parent, Sprite _prefab, Rect _rect, Rect _rectTarget, Vector2 _pivot)
		{
			RectTransform newTransform = _parent.AddComponent<RectTransform>();
			_parent.AddComponent<CanvasRenderer>();
			Image srcImage = _parent.AddComponent<Image>() as Image;
			Sprite sprite = Sprite.Create(_prefab.texture, _rect, _pivot);
			if ((_rectTarget.width != 0) && (_rectTarget.height != 0))
			{
				newTransform.sizeDelta = new Vector2(_rectTarget.width, _rectTarget.height);
			}
			srcImage.sprite = sprite;
			return sprite;
		}

		// ---------------------------------------------------
		/**
		 @brief Check if between two points there is no obstacle
		 */
		public static bool CheckFreePath(Vector3 _goalPosition, Vector3 _originPosition, params int[] _masks)
		{
			Vector3 fwd = new Vector3(_goalPosition.x - _originPosition.x, _goalPosition.y - _originPosition.y, _goalPosition.z - _originPosition.z);
			float distanceTotal = fwd.sqrMagnitude;
			fwd.Normalize();
			Ray ray = new Ray();
			ray.direction = fwd;
			RaycastHit hitCollision = new RaycastHit();

			int layerMask = Physics.DefaultRaycastLayers;
			for (int i = 0; i < _masks.Length; i++)
			{
				layerMask = layerMask & _masks[i];
			}
			if (Physics.Raycast(_originPosition, fwd, out hitCollision, Mathf.Infinity, layerMask))
			{
				// var distanceToGround = hitCollision.distance;
				if (Vector3.Distance(hitCollision.point, _originPosition) < distanceTotal)
				{
					return false;
				}
			}

			return true;
		}

		// ---------------------------------------------------
		/**
		 @brief Gets the collision point between two positions, zero if there is no collision
		 */
		public static Vector3 GetCollisionPoint(Vector3 _goalPosition, Vector3 _originPosition, params int[] _masks)
		{
			Vector3 fwd = new Vector3(_goalPosition.x - _originPosition.x, _goalPosition.y - _originPosition.y, _goalPosition.z - _originPosition.z);
			float distanceTotal = fwd.magnitude;
			fwd.Normalize();
			Ray ray = new Ray();
			ray.direction = fwd;
			RaycastHit hitCollision = new RaycastHit();

			int layerMask = Physics.DefaultRaycastLayers;
			for (int i = 0; i < _masks.Length; i++)
			{
				layerMask = layerMask & _masks[i];
			}
			if (Physics.Raycast(_originPosition, fwd, out hitCollision, Mathf.Infinity, layerMask))
			{
				if (Vector3.Distance(hitCollision.point, _originPosition) < distanceTotal)
				{
					return hitCollision.point;
				}
			}

			return Vector3.zero;
		}

		// ---------------------------------------------------
		/**
		 @brief ClonePoint
		 */
		public static Vector3 ClonePoint(Vector3 _position)
		{
			return new Vector3(_position.x, _position.y, _position.z);
		}

		// ---------------------------------------------------
		/**
		 @brief ClonePoint
		 */
		public static Vector2 ClonePoint(Vector2 _position)
		{
			return new Vector2(_position.x, _position.y);
		}

		// ---------------------------------------------------
		/**
		 @brief We get the collided object through the forward vector
		 */
		public static GameObject GetCollidedObjectByRay(Vector3 _origin, Vector3 _forward)
		{
			Vector3 fwd = ClonePoint(_forward);
			fwd.Normalize();
			Ray ray = new Ray();
			ray.direction = _forward;
			RaycastHit hitCollision = new RaycastHit();

			if (Physics.Raycast(_origin, fwd, out hitCollision))
			{
				return hitCollision.collider.gameObject;
			}

			return null;
		}


		// ---------------------------------------------------
		/**
		 @brief We get the whole RaycastHit information of the collision, with the mask to ignore
		 */
		public static RaycastHit GetRaycastHitInfoByRay(Vector3 _origin, Vector3 _forward, params string[] _masksToIgnore)
		{
			Vector3 fwd = ClonePoint(_forward);
			fwd.Normalize();
			RaycastHit hitCollision = new RaycastHit();

			int layerMask = Physics.DefaultRaycastLayers;
			if (_masksToIgnore != null)
			{
				for (int i = 0; i < _masksToIgnore.Length; i++)
				{
					layerMask |= ~(1 << LayerMask.NameToLayer(_masksToIgnore[i]));
				}
			}
			Physics.Raycast(_origin, fwd, out hitCollision, Mathf.Infinity, layerMask);
			return hitCollision;
		}

		// ---------------------------------------------------
		/**
		 @brief We get the whole RaycastHit information of the collision, with the mask to consider
		 */
		public static RaycastHit GetRaycastHitInfoByRayWithMask(Vector3 _origin, Vector3 _forward, params string[] _masksToConsider)
		{
			Vector3 fwd = ClonePoint(_forward);
			fwd.Normalize();
			RaycastHit hitCollision = new RaycastHit();

			int layerMask = 0;
			if (_masksToConsider != null)
			{
				for (int i = 0; i < _masksToConsider.Length; i++)
				{
					layerMask |= (1 << LayerMask.NameToLayer(_masksToConsider[i]));
				}
			}
			if (layerMask == 0)
			{
				Physics.Raycast(_origin, fwd, out hitCollision, Mathf.Infinity);
			}
			else
			{
				Physics.Raycast(_origin, fwd, out hitCollision, Mathf.Infinity, layerMask);
			}
			return hitCollision;
		}

		// ---------------------------------------------------
		/**
		 @brief We get the collided object between 2 points
		 */
		public static GameObject GetCollidedObjectByRayTarget(Vector3 _goalPosition, Vector3 _originPosition, params int[] _masks)
		{
			Vector3 fwd = new Vector3(_goalPosition.x - _originPosition.x, _goalPosition.y - _originPosition.y, _goalPosition.z - _originPosition.z);
			float distanceTotal = fwd.sqrMagnitude;
			fwd.Normalize();
			Ray ray = new Ray();
			ray.direction = fwd;
			RaycastHit hitCollision = new RaycastHit();

			int layerMask = Physics.DefaultRaycastLayers;
			for (int i = 0; i < _masks.Length; i++)
			{
				layerMask = layerMask & _masks[i];
			}
			if (Physics.Raycast(_originPosition, fwd, out hitCollision, Mathf.Infinity, layerMask))
			{
				return hitCollision.collider.gameObject;
			}

			return null;
		}

		// ---------------------------------------------------
		/**
		 @brief We get the collided object for a forward vector
		 */
		public static GameObject GetCollidedObjectByRayForward(Vector3 _origin, Vector3 _forward, params int[] _masks)
		{
			Vector3 fwd = ClonePoint(_forward);
			fwd.Normalize();
			Ray ray = new Ray();
			ray.direction = _forward;
			RaycastHit hitCollision = new RaycastHit();

			int layerMask = Physics.DefaultRaycastLayers;
			for (int i = 0; i < _masks.Length; i++)
			{
				layerMask = layerMask & _masks[i];
			}
			if (Physics.Raycast(_origin, fwd, out hitCollision, Mathf.Infinity, layerMask))
			{
				return hitCollision.collider.gameObject;
			}

			return null;
		}

		// ---------------------------------------------------
		/**
		 @brief We get the collision point for a forward vector
		 */
		public static Vector3 GetCollidedPointByRayForward(Vector3 _origin, Vector3 _forward, params int[] _masks)
		{
			Vector3 fwd = ClonePoint(_forward);
			fwd.Normalize();
			Ray ray = new Ray();
			ray.direction = _forward;
			RaycastHit hitCollision = new RaycastHit();

			int layerMask = Physics.DefaultRaycastLayers;
			for (int i = 0; i < _masks.Length; i++)
			{
				layerMask = layerMask & _masks[i];
			}
			if (Physics.Raycast(_origin, fwd, out hitCollision, Mathf.Infinity, layerMask))
			{
				return hitCollision.point;
			}

			return Vector3.zero;
		}

		// -------------------------------------------
		/* 
		 * We apply a material on all the hirarquy of objects
		 */
		public static void ApplyMaterialOnImages(GameObject _go, Material _material)
		{
			foreach (Transform child in _go.transform)
			{
				ApplyMaterialOnImages(child.gameObject, _material);
			}
			if (_go.GetComponent<Image>() != null)
			{
				_go.GetComponent<Image>().material = _material;
			}

			if (_go.GetComponent<Text>() != null)
			{
				_go.GetComponent<Text>().material = _material;
			}
		}

		// -------------------------------------------
		/* 
		 * Check if the objects is visible in the camera's frustum
		 */
		public static bool IsVisibleFrom(Bounds _bounds, Camera _camera)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);
			return GeometryUtility.TestPlanesAABB(planes, _bounds);
		}

		// -------------------------------------------
		/* 
		 * Move with control keys, with rigid body if there is one available or forcing the position
		 */
		public static void MoveAroundOrientedRigidbody(Transform _target, Vector3 _forward, Vector3 _right, float _speedMovement)
		{
			float moveForward = _speedMovement * Time.smoothDeltaTime * Input.GetAxis("Vertical");
			float moveLeft = _speedMovement * Time.smoothDeltaTime * Input.GetAxis("Horizontal");

			Vector3 newPosition = _target.position + (_forward * moveForward) + (_right * moveLeft);
			Vector3 normal = newPosition - _target.position;
			normal.Normalize();
			Vector3 newVelocity = normal * _speedMovement;
			if (!_target.GetComponent<Rigidbody>().isKinematic)
			{
				_target.GetComponent<Rigidbody>().velocity = new Vector3(newVelocity.x, _target.GetComponent<Rigidbody>().velocity.y, newVelocity.z);
			}
			else
			{
				_target.position = newPosition;
			}
		}

		// -------------------------------------------
		/* 
		 * Get the bounds of game object
		 */
		public static Bounds CalculateBounds(GameObject _gameObject)
		{
			Bounds bounds = CalculateBoundsThroughCollider(_gameObject);
			if (bounds.size == Vector3.zero)
			{
				bounds = CalculateBoundsThroughRenderer(_gameObject);
			}
			if (bounds.size == Vector3.zero)
			{
				bounds = CalculateBoundsThroughMesh(_gameObject);
			}
			return bounds;
		}

		// -------------------------------------------
		/* 
		 * Get the bounds through renderer
		 */
		public static Bounds CalculateBoundsThroughRenderer(GameObject _gameObject)
		{
			Renderer[] meshRenderers = _gameObject.GetComponentsInChildren<Renderer>();

			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

			foreach (Renderer bc in meshRenderers)
			{
				bounds.Encapsulate(bc.bounds);
			}

			return bounds;
		}

		// -------------------------------------------
		/* 
		 * Get the bounds through collider
		 */
		public static Bounds CalculateBoundsThroughCollider(GameObject _gameObject)
		{
			Collider[] colliders = _gameObject.GetComponentsInChildren<Collider>();

			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

			foreach (Collider bc in colliders)
			{
				bounds.Encapsulate(bc.bounds);
			}

			return bounds;
		}

		// -------------------------------------------
		/* 
		 * Get the bounds through mesh
		 */
		public static Bounds CalculateBoundsThroughMesh(GameObject _gameObject)
		{
			MeshRenderer[] meshRenderers = _gameObject.GetComponentsInChildren<MeshRenderer>();

			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

			foreach (MeshRenderer bc in meshRenderers)
			{
				bounds.Encapsulate(bc.bounds);
			}

			return bounds;
		}

		// -------------------------------------------
		/* 
		 * Check if there is a collider
		 */
		public static bool IsThereABoxCollider(GameObject _gameObject)
		{
			Collider[] colliders = _gameObject.GetComponentsInChildren<BoxCollider>();

			return colliders.Length > 0;
		}


		// -------------------------------------------
		/* 
		 * Return the rect container
		 */
		public static Rect GetCornersRectTransform(RectTransform _rectTransform)
		{
			Vector3[] corners = new Vector3[4];
			_rectTransform.GetWorldCorners(corners);
			Rect rec = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
			return rec;
		}



		// -------------------------------------------
		/* 
		 * Will look fot the gameobject in the childs
		 */
		public static bool FindGameObjectInChilds(GameObject _go, GameObject _target)
		{
			if (_go == _target)
			{
				return true;
			}
			bool output = false;
			foreach (Transform child in _go.transform)
			{
				output = output || FindGameObjectInChilds(child.gameObject, _target);
			}
			return output;
		}


		// -------------------------------------------
		/* 
		 * Will generate a random string
		 */
		public static string RandomCodeGeneration(string _idUser)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var stringChars = new char[8];
			var random = new System.Random();

			for (int i = 0; i < stringChars.Length; i++)
			{
				stringChars[i] = chars[random.Next(chars.Length)];
			}

			string finalString = new String(stringChars) + "_" + _idUser;
			return finalString;
		}

		// -------------------------------------------
		/* 
		 * Copy to the clipboard
		 */
		public static string Clipboard
		{
			get { return GUIUtility.systemCopyBuffer; }
			set { GUIUtility.systemCopyBuffer = value; }
		}
		
		// -------------------------------------------
		/* 
		 * GetFiles
		 */
		public static string[] GetFiles(string _path, string _searchPattern, SearchOption _searchOption)
		{
			string[] searchPatterns = _searchPattern.Split('|');
			List<string> files = new List<string>();
			foreach (string sp in searchPatterns)
			{
				files.AddRange(System.IO.Directory.GetFiles(_path, sp, _searchOption));
			}
			files.Sort();
			return files.ToArray();
		}

		// -------------------------------------------
		/* 
		 * GetFiles
		 */
		public static FileInfo[] GetFiles(DirectoryInfo _path, string _searchPattern, SearchOption _searchOption)
		{
			string[] searchPatterns = _searchPattern.Split('|');
			List<FileInfo> files = new List<FileInfo>();
			foreach (string sp in searchPatterns)
			{
				files.AddRange(_path.GetFiles(sp, _searchOption));
			}
			return files.ToArray();
		}
	}
}