﻿using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public bool inControl = true;
	public bool canWarp = true;
	public float speed = 5f;
	
	public KeyCode upKey = KeyCode.W;
	public KeyCode downKey = KeyCode.S;
	public KeyCode leftKey = KeyCode.A;
	public KeyCode rightKey = KeyCode.D;
	public KeyCode attackKey = KeyCode.J;
	public KeyCode itemKey = KeyCode.K;
	public KeyCode warpCentricKey = KeyCode.X;
	public KeyCode warpExteriorKey = KeyCode.Z;
	public KeyCode mapViewKey = KeyCode.M;
	public KeyCode replayKey = KeyCode.R;
	public KeyCode quitKey = KeyCode.Q;
	
	public Rigidbody2D rigidbody2D;
	public BaseWeaponController currentWeapon;
	public BaseItemController currentItem;

	public Direction facing;
	public Vector2 velocity;

	public void Update() {

		if (Input.GetKey(replayKey)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		if (Input.GetKey(quitKey)) {
#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
		}
		
		velocity = Vector2.zero;
		
		if (inControl) {
			float vertical = (Input.GetKey(upKey) ? 1 : 0) + (Input.GetKey(downKey) ? -1 : 0);
			float horizontal = (Input.GetKey(rightKey) ? 1 : 0) + (Input.GetKey(leftKey) ? -1 : 0);

			Direction curr = facing;
			
			if (vertical > 0) curr = Direction.UP;
			else if (vertical < 0) curr = Direction.DOWN;
			
			if (horizontal > 0) curr = Direction.RIGHT;
			else if (horizontal < 0) curr = Direction.LEFT;
			
			Turn(curr);
			
			velocity = new Vector2(horizontal, vertical).normalized * speed;

			if (currentWeapon && currentWeapon.isAttacking) {
				if (Input.GetKeyUp(attackKey)) {
					if (currentWeapon != null) currentWeapon.OnRelease();
				}
			} else if (currentItem && currentItem.isUsing) {
				if (Input.GetKeyUp(itemKey)) {
					if (currentItem != null) currentItem.OnRelease();
				}
			} else {
				if (Input.GetKeyDown(attackKey)) {
					if (currentWeapon != null) currentWeapon.OnPress();
				} else if (Input.GetKeyDown(itemKey)) {
					if (currentItem != null) currentItem.OnPress();
				}
			}

			if (WarpManager.Instance != null) // code specific to the warp part of game
			{
				if (canWarp) {
					if (Input.GetKeyDown(warpCentricKey)) {
						WarpManager.Instance.WarpCentricAll();
					}
					if (Input.GetKeyDown(warpExteriorKey)) {
						WarpManager.Instance.WarpExteriorAll();
					}
				}
				if (Input.GetKey(mapViewKey))
				{
					WarpManager.Instance.MapView();
				}
				else
				{
					WarpManager.Instance.GameView();
				}
			}
		}

		rigidbody2D.velocity = velocity;
	}

	public void Turn(Direction direction) {
		if (direction == facing) return;
		facing = direction;
		Vector3 eulerAngle = Vector3.one;
		switch (direction) {
			case Direction.UP: eulerAngle = new Vector3(0, 0, 0); break;
			case Direction.DOWN: eulerAngle = new Vector3(0, 0, 180); break;
			case Direction.LEFT: eulerAngle = new Vector3(0, 0, 90); break;
			case Direction.RIGHT: eulerAngle = new Vector3(0, 0, -90); break;
		}
		
		transform.rotation = Quaternion.Euler(eulerAngle);
	}
}

public enum Direction {
	UP,
	DOWN,
	LEFT,
	RIGHT
}