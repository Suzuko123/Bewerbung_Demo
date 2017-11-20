using UnityEngine;

//Author: William Rapprich
//Last edited: 20.11.2017 by William

/// <summary>
/// Handles player behaviour for floor with IcyFloor layer.
/// </summary>
public class IcyFloorHandler : MonoBehaviour
{
	Player player;

    [SerializeField]float boxCastHeightDifference = 0.2f;
    [SerializeField]float checkDistance = 0.1f;
    [SerializeField]float minimumBlockingSlopeDegrees = 40f;
    [SerializeField]float slidingSpeedMultiplier = 1.5f;

	void Start()
	{
		player = Player.Instance;
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("IcyFloor"))
        {
            if (player.IsSliding)
            {
                Vector3 dir = new Vector3(player.MovementInput.x, 0f, player.MovementInput.y); //make Vector3 with movement input

                /* boxcast in the direction the player is moving: 
                    - height is smaller than character controller height by boxCastHeightDifference
                    - width is half of the charactercontroller radius */
                RaycastHit[] raycastHits = Physics.BoxCastAll(
                                                                transform.position + Vector3.up * player.GetHeight() / 2f, 
                                                                new Vector3(player.GetRadius()/2f, 
                                                                (player.GetHeight() - boxCastHeightDifference)/2f, 
                                                                player.GetRadius()/2f), 
                                                                dir, 
                                                                Quaternion.LookRotation(dir)
                                                             );
                
                foreach(RaycastHit raycastHit in raycastHits)
                {
                    if (raycastHit.point != Vector3.zero //make sure it doesn't collide at the start of the boxcast
                        && raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacles")
                        && raycastHit.distance < player.GetRadius()/2f + checkDistance
                        && Vector3.Dot(raycastHit.normal, dir) < 0 //whether obstacle is facing towards the player
                        && Mathf.Acos(raycastHit.normal.y) * Mathf.Rad2Deg >= minimumBlockingSlopeDegrees) //surface is at least as steep as minimumBlockingSlopeDegrees
                    {
                        player.IsSliding = false;
                        player.MovementInput = Vector3.zero;
                        player.speedMultiplier = 1f;
                        break;
                    }
                }
            }
            else if (player.GetMovement() != Vector2.zero)
            {
                player.IsSliding = true;
                Transform trans = hit.collider.transform; //memorize for easier access
                //make 2D vectors for the icyfloor object orientation axes
                Vector2 forwardVector2 = new Vector2(trans.forward.x, trans.forward.z);
                Vector2 rightVector2 = new Vector2(trans.right.x, trans.right.z);

                //dot products for all axes and the player movement input to determine dominant direction
                float forward = Vector3.Dot(player.MovementInput, forwardVector2);
                float backward = Vector3.Dot(player.MovementInput, -1 * forwardVector2);
                float right = Vector3.Dot(player.MovementInput, rightVector2);
                float left = Vector3.Dot(player.MovementInput, -1 * rightVector2);

                float highest = Mathf.Max(forward, backward, right, left); //dominant axis

                // switch dominant axis (can't switch floats, thus if-else block) 
                if (highest == forward)
                {
                    //move player along forward axis
                    player.MovementInput = forwardVector2 * player.MovementInput.magnitude;
                }
                else if (highest == backward)
                {
                    //move player along backward axis
                    player.MovementInput = -1 * forwardVector2 * player.MovementInput.magnitude;
                }
                else if (highest == right)
                {
                    //move player along right axis
                    player.MovementInput = rightVector2 * player.MovementInput.magnitude;
                }
                else if (highest == left)
                {
                    //move player along left axis
                    player.MovementInput = -1 * rightVector2 * player.MovementInput.magnitude;
                }

                //Make player face new direction and adjust speed
                transform.localRotation = Quaternion.LookRotation(new Vector3(player.MovementInput.x, 0f, player.MovementInput.y), Vector3.up);
                player.speedMultiplier = slidingSpeedMultiplier;
            }
        }
        else if (player.IsSliding && hit.collider.gameObject.layer == LayerMask.NameToLayer("Floor")) //entering normal floor when sliding
        {
            player.IsSliding = false;
            player.MovementInput = Vector3.zero;
            player.speedMultiplier = 1f;
        }
    }
}
