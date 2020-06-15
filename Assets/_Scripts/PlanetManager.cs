using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public static PlanetManager instance;

    public float gravity = -5f;

    private void Awake()
    {
        instance = this;
    }

    public void applyGravity(Transform rock)
    {
        Vector3 gravityUp = (rock.position - transform.position).normalized;
        Vector3 rockUp = rock.up;

        rock.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);

        Quaternion targetRotation = Quaternion.FromToRotation(rockUp, gravityUp) * rock.rotation;
      //  rock.rotation = Quaternion.Slerp(rock.rotation, targetRotation, 50 * Time.deltaTime);
    }
}
