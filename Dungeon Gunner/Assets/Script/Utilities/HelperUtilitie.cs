using System.Collections;
using UnityEngine;

public static class HelperUtilitie
{
    public static Camera mainCamera;

    /// <summary>
    /// Get the mouse world position
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        // clamp mouse position to screen size
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPostion = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPostion.z = 0f;

        return worldPostion;
    }

    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLowerBounds, out Vector2Int cameraWorldPositionUpperBOunds, Camera camera)
    {
        Vector3 worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewportTopRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        cameraWorldPositionLowerBounds = new Vector2Int((int)worldPositionViewportBottomLeft.x, (int)worldPositionViewportBottomLeft.y);
        cameraWorldPositionUpperBOunds = new Vector2Int((int)worldPositionViewportTopRight.x, (int)worldPositionViewportTopRight.y);
    }

    /// <summary>
    /// Get the angle in degrees from a direction vector
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float degrees = radians * Mathf.Rad2Deg;

        return degrees;
    }

    /// <summary>
    ///  Get the direction vector from an angle in degrees
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    /// <summary>
    /// Get aimDirection enum value from the pased in angleDegrees.
    /// </summary>
    /// <param name="angleDegrees"></param>
    /// <returns></returns>
    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection;

        if (22f <= angleDegrees && angleDegrees <= 67f)
        {
            aimDirection = AimDirection.UpRight;
        }
        else if (67f < angleDegrees && angleDegrees <= 112f)
        {
            aimDirection = AimDirection.Up;
        }
        else if (112 < angleDegrees && angleDegrees <= 158f)
        {
            aimDirection = AimDirection.UpLeft;
        }
        else if ((158f < angleDegrees && angleDegrees <= 180f) || (-180 < angleDegrees && angleDegrees <= -135f))
        {
            aimDirection = AimDirection.Left;
        }
        else if (-135f < angleDegrees && angleDegrees <= -45f)
        {
            aimDirection = AimDirection.Down;
        }
        else if ((-45f < angleDegrees && angleDegrees <= 0f) || (0f < angleDegrees && angleDegrees < 22f))
        {
            aimDirection = AimDirection.Right;
        }
        else
        {
            aimDirection = AimDirection.Right;
        }

        return aimDirection;
    }

    /// <summary>
    /// Convert the linear volume scale to decibels
    /// </summary>
    /// <param name="linear"></param>
    /// <returns></returns>
    public static float LinearToDecibels(int linear)
    {
        float linearScaleRange = 20f;

        // formula to convert from the linear scale to the logarithmic decibel scale
        return Mathf.Log10((float)linear / linearScaleRange) * 20f;
    }

    /// <summary>
    /// empty string debug check
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldName"></param>
    /// <param name="stringToCheck"></param>
    /// <returns></returns>
    public static bool ValidateCheckEmptyString(UnityEngine.Object thisObject, string fieldName, string stringToCheck)
    {
        if(stringToCheck == "")
        {
            Debug.Log("is empty and must contain a value in object" + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    /// <summary>
    /// null value debug check 
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldName"></param>
    /// <param name="objectTocheck"></param>
    /// <returns></returns>
    public static bool ValidateCheckNullValue(UnityEngine.Object thisObject, string fieldName, UnityEngine.Object objectTocheck)
    {
        if(objectTocheck == null)
        {
            Debug.Log(fieldName + "is null and must contain a value in object" + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    public static bool ValidateCheckEnumerableValues(UnityEngine.Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if(enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + "is null in object " + thisObject.name.ToString());
            return true;
        }

        foreach(var item in enumerableObjectToCheck)
        {
            if(item == null)
            {
                Debug.Log(fieldName + "has null values in object" + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if(count == 0)
        {
            Debug.Log(fieldName + "has no values in object" + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    /// <summary>
    /// positive value debug check - if zero is allowed set isZeroAllowed to true. Returns true if there is an error
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldName"></param>
    /// <param name="valueToCheck"></param>
    /// <param name="isZeroAllowed"></param>
    /// <returns></returns>
    public static bool ValidateCheckPositiveValue(UnityEngine.Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in object" + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object" + thisObject.name.ToString());
                error = true;
            }
        }
        return error;
    }

    /// <summary>
    /// positive value debug check - if zero is allowed set isZeroAllowed to true. Return true if is an error.
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldName"></param>
    /// <param name="valueToCheck"></param>
    /// <param name="isZeroAllowed"></param>
    /// <returns></returns>
    public static bool ValidateCheckPositiveValue(UnityEngine.Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fieldName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fieldName + " must contain a positive value in object" + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    /// <summary>
    /// positive range debug check - set isZeroAllowed to true if the min and max range values can both be zero. Returns true if there is an error
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldNameMinimum"></param>
    /// <param name="valueToCheckMinimum"></param>
    /// <param name="fieldNameMaximum"></param>
    /// <param name="valueToCheckMaximum"></param>
    /// <param name="isZeroAllowed"></param>
    /// <returns></returns>
    public  static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float valueToCheckMinimum, string fieldNameMaximum,
        float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to " + fieldNameMaximum + " in object " + thisObject.name.ToString());
            error = true;

        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;

        return error;
    }

    /// <summary>
    /// positive range debug check - set isZeroAllowed to true if the min and ma range values can both be zero. Return true if there is an error
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="fieldNameMinimum"></param>
    /// <param name="valueToCheckMinimum"></param>
    /// <param name="fieldNameMaximum"></param>
    /// <param name="valueToCheckMaximum"></param>
    /// <param name="isZeroAllowed"></param>
    /// <returns></returns>
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, int valueToCheckMinimum, string fieldNameMaximum,
       int valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fieldNameMinimum + " must be less than or equal to " + fieldNameMaximum + " in object " + thisObject.name.ToString());
            error = true;

        }

        if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;

        if (ValidateCheckPositiveValue(thisObject, fieldNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;

        return error;
    }

    /// <summary>
    /// Get the nearest spawn position to the player
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3 nearestSpawnPosition = new Vector3(1000f, 1000f, 0f);

        // loop through room spawn positions
        foreach(Vector2Int spawnPostionGrid in currentRoom.spawnPositionArray)
        {
            // convert the spawn grid positions to world positions
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPostionGrid);

            if(Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(nearestSpawnPosition, playerPosition))
            {
                nearestSpawnPosition = spawnPositionWorld;
            }
        }

        return nearestSpawnPosition;

    }
}
