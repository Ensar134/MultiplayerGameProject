using System;
using Mirror;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : NetworkBehaviour
	{
		
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool oneShoot;
		public bool BurstShoot;
		public bool Reloading;
		public bool PicupGun;
		public bool repair;
		public bool DropOdun;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}
		public void OnBurstShoot(InputValue value)
		{
			BurstShootInput(Convert.ToBoolean(value.Get<float>()));			
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}
		public void OnReloading(InputValue value)
		{
			ReloadingInput(value.isPressed);
		}
		public void OnPicupGun(InputValue value)
		{
			PicupGunInput(value.isPressed);
		}
		public void OnDropOdun(InputValue value)
		{
			DropOdunInput(value.isPressed);
		}
		public void OnRepair(InputValue value)
		{
			RepairInput(value.isPressed);
		}
		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}
		public void OnOneShoot(InputValue value)
		{
			OneShootInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}
		public void OneShootInput(bool newShootState)
		{
			oneShoot = newShootState;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}
		public void ReloadingInput(bool newReloadState)
		{
			Reloading = newReloadState;
		}public void DropOdunInput(bool newReloadState)
		{
			DropOdun = newReloadState;
		}
		public void PicupGunInput(bool newReloadState)
		{
			PicupGun = newReloadState;
		}
		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		public void BurstShootInput(bool newShootState)
		{
			BurstShoot = newShootState;
		}
		public void RepairInput(bool newRepairState)
		{
			repair = newRepairState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}