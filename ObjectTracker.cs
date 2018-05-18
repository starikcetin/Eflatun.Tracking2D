using System.Collections.Generic;
using starikcetin.UnityCommon.Utils.Calculation;
using starikcetin.UnityCommon.Utils.CodePatterns;
using UnityEngine;

namespace starikcetin.Tracking2D
{
    public class ObjectTracker : GlobalSingleton<ObjectTracker>
    {
        //prevent init
        protected ObjectTracker()
        {
        }

        #region Record Keeping

        public void RegisterObject(GameObject toRegister, GameObject prefab)
        {
            var renderer = toRegister.GetComponent<SpriteRenderer>();
            if (renderer)
            {
                IList<Vector2> localConvexHull = ConvexHullDatabase.Instance.GetLocalConvexHull(prefab);

                var newData = new TrackedObjectData(toRegister, renderer, localConvexHull);
                ObjectTrackerDatabase.Add(toRegister, newData);
            }
        }

        public void UnregisterObject(GameObject toUnregister)
        {
            ObjectTrackerDatabase.Remove(toUnregister);
        }

        #endregion

        #region Functionalities (Multi Returners)

        /// <summary>
        /// Similar to RaycastAll.
        /// </summary>
        public IList<GameObject> ByPoint_All(Vector2 point, int layerMask = MathUtils.LayerMaskAll)
        {
            List<TrackedObjectData> allDatas = ObjectTrackerDatabase.AllDatas;

            //--- Preparation: Determine if we need to check for layers.  ---
            var checkForLayers = layerMask != MathUtils.LayerMaskAll;

            //--- Iterate: Test all datas. ---
            var results = new List<GameObject>();

            var count = allDatas.Count;
            for (var i = 0; i < count; i++)
            {
                var item = allDatas[i];

                //--- Test: Run the point test. ---
                if (Test_ByPoint(item, point, layerMask, checkForLayers))
                {
                    results.Add(item.GameObject);
                }
            }

            //--- Finally: Return all. ---
            return results;
        }

        /// <summary>
        /// Similar to OverlapAreaAll.
        /// </summary>
        public IList<GameObject> ByBox_All(Vector2 startPoint, Vector2 endPoint, int layerMask = MathUtils.LayerMaskAll)
        {
            List<TrackedObjectData> allDatas = ObjectTrackerDatabase.AllDatas;

            //--- Preparation: Determine if we need to check for layers.  ---
            //---              Determine max and min points of test area. ---
            var checkForLayers = layerMask != MathUtils.LayerMaskAll;

            var min = new Vector2();
            var max = new Vector2();

            if (startPoint.x > endPoint.x)
            {
                //start point's X is bigger.
                max.x = startPoint.x;
                min.x = endPoint.x;
            }
            else
            {
                //end point's X is bigger or equal.
                max.x = endPoint.x;
                min.x = startPoint.x;
            }

            if (startPoint.y > endPoint.y)
            {
                //start point's Y is bigger.
                max.y = startPoint.y;
                min.y = endPoint.y;
            }
            else
            {
                //end point's Y is bigger or equal.
                max.y = endPoint.y;
                min.y = startPoint.y;
            }

            //--- Iterate: Test all datas. ---
            IList<GameObject> results = new List<GameObject>();

            var count = allDatas.Count;
            for (var i = 0; i < count; i++)
            {
                var item = allDatas[i];

                //--- Test: Run the box test. ---
                if (Test_ByBox(item, min, max, layerMask, checkForLayers))
                {
                    results.Add(item.GameObject);
                }
            }

            //--- Finally: Return all. ---
            return results;
        }

        /// <summary>
        /// Similar to OverlapCircleAll.
        /// </summary>
        public IList<GameObject> ByCircle_All(Vector2 center, float radius, int layerMask = MathUtils.LayerMaskAll)
        {
            List<TrackedObjectData> allDatas = ObjectTrackerDatabase.AllDatas;

            //--- Preparation: Determine if we need to check for layers.  ---
            //---              Calculate squared radius.                  ---
            //---              Determine max and min points of test area. ---
            var checkForLayers = layerMask != MathUtils.LayerMaskAll;
            var sqrRadius = radius * radius;
            var min = new Vector2(center.x - radius, center.y - radius);
            var max = new Vector2(center.x + radius, center.y + radius);

            //--- Iterate: Test all datas. ---
            IList<GameObject> results = new List<GameObject>();

            var count = allDatas.Count;
            for (var i = 0; i < count; i++)
            {
                var item = allDatas[i];

                //--- Test: Run the circle test. ---
                if (Test_ByCircle(item, center, sqrRadius, min, max, layerMask, checkForLayers))
                {
                    results.Add(item.GameObject);
                }
            }

            //--- Finally: Return all. ---
            return results;
        }

        #endregion

        #region Functionalities (Single Returners)

        /// <summary>
        /// Similar to Raycast.
        /// </summary>
        public GameObject ByPoint_Single(Vector2 point, int layermask = MathUtils.LayerMaskAll)
        {
            List<TrackedObjectData> allDatas = ObjectTrackerDatabase.AllDatas;
            var pickedData = Pick_ByPoint(allDatas, point, layermask);
            return GetGameObjectOrNull(pickedData);
        }

        /// <summary>
        /// Similar to OverlapArea.
        /// </summary>
        public GameObject ByBox_Single(Vector2 startPoint, Vector2 endPoint, int layerMask = MathUtils.LayerMaskAll)
        {
            //--- Preparation: Determine max and min points of test area. ---
            var min = new Vector2();
            var max = new Vector2();

            if (startPoint.x > endPoint.x)
            {
                //start point's X is bigger.
                max.x = startPoint.x;
                min.x = endPoint.x;
            }
            else
            {
                //end point's X is bigger or equal.
                max.x = endPoint.x;
                min.x = startPoint.x;
            }

            if (startPoint.y > endPoint.y)
            {
                //start point's Y is bigger.
                max.y = startPoint.y;
                min.y = endPoint.y;
            }
            else
            {
                //end point's Y is bigger or equal.
                max.y = endPoint.y;
                min.y = startPoint.y;
            }

            //--- Pick: Pick by box. ---
            List<TrackedObjectData> allDatas = ObjectTrackerDatabase.AllDatas;
            var pickedData = Pick_ByBox(allDatas, min, max, layerMask);
            return GetGameObjectOrNull(pickedData);
        }

        /// <summary>
        /// Similar to OverlapCircle.
        /// </summary>
        public GameObject ByCircle_Single(Vector2 center, float radius, int layerMask = MathUtils.LayerMaskAll)
        {
            List<TrackedObjectData> allDatas = ObjectTrackerDatabase.AllDatas;
            var pickedData = Pick_ByCircle(allDatas, center, radius, layerMask);
            return GetGameObjectOrNull(pickedData);
        }

        #endregion

        #region Functionalities (Non-Alloc Multi Returners)

        /// <summary>
        /// Similar to RaycastNonAlloc
        /// </summary>
        public int ByPoint_AllNonAlloc(GameObject[] outputArray, Vector2 point, int layerMask = MathUtils.LayerMaskAll)
        {
            List<TrackedObjectData> allDatas = ObjectTrackerDatabase.AllDatas;

            //--- Preparation: Determine if we need to check for layers.  ---
            //---              Get output capacity.                       ---
            var checkForLayers = layerMask != MathUtils.LayerMaskAll;
            var outputCapacity = outputArray.Length;

            //--- Iterate: Test all datas. ---
            var resultCount = 0;
            var count = allDatas.Count;
            for (var i = 0; i < count; i++)
            {
                var item = allDatas[i];

                //--- Test: Run the point test. ---
                if (Test_ByPoint(item, point, layerMask, checkForLayers))
                {
                    //--- Finally: Assign to current index. ---
                    outputArray[resultCount] = item.GameObject;
                    resultCount++;

                    // Check if we hit the capacity of output array.
                    if (resultCount == outputCapacity)
                    {
                        break;
                    }
                }
            }

            //--- Finally: Return result count. ---
            return resultCount;
        }

        /// <summary>
        /// Similar to OverlapAreaNonAlloc
        /// </summary>
        public int ByBox_AllNonAlloc(GameObject[] outputArray, Vector2 startPoint, Vector2 endPoint,
            int layerMask = MathUtils.LayerMaskAll)
        {
            List<TrackedObjectData> allDatas = ObjectTrackerDatabase.AllDatas;

            //--- Preparation: Determine if we need to check for layers.  ---
            //---              Get output capacity.                       ---
            //---              Determine max and min points of test area. ---
            var checkForLayers = layerMask != MathUtils.LayerMaskAll;
            var outputCapacity = outputArray.Length;

            var min = new Vector2();
            var max = new Vector2();

            if (startPoint.x > endPoint.x)
            {
                //start point's X is bigger.
                max.x = startPoint.x;
                min.x = endPoint.x;
            }
            else
            {
                //end point's X is bigger or equal.
                max.x = endPoint.x;
                min.x = startPoint.x;
            }

            if (startPoint.y > endPoint.y)
            {
                //start point's Y is bigger.
                max.y = startPoint.y;
                min.y = endPoint.y;
            }
            else
            {
                //end point's Y is bigger or equal.
                max.y = endPoint.y;
                min.y = startPoint.y;
            }

            //--- Iterate: Test all datas. ---
            var resultCount = 0;
            var count = allDatas.Count;
            for (var i = 0; i < count; i++)
            {
                var item = allDatas[i];

                //--- Test: Run the box test. ---
                if (Test_ByBox(item, min, max, layerMask, checkForLayers))
                {
                    //--- Finally: Assign to current index. ---
                    outputArray[resultCount] = item.GameObject;
                    resultCount++;

                    // Check if we hit the capacity of output array.
                    if (resultCount == outputCapacity)
                    {
                        break;
                    }
                }
            }

            //--- Finally: Return result count. ---
            return resultCount;
        }

        /// <summary>
        /// Similar to OverlapCircleNonAlloc
        /// </summary>
        public int ByCircle_AllNonAlloc(GameObject[] outputArray, Vector2 center, float radius,
            int layerMask = MathUtils.LayerMaskAll)
        {
            List<TrackedObjectData> allDatas = ObjectTrackerDatabase.AllDatas;

            //--- Preparation: Determine if we need to check for layers.  ---
            //---              Calculate squared radius.                  ---
            //---              Get output capacity.                       ---
            //---              Determine max and min points of test area. ---
            var checkForLayers = layerMask != MathUtils.LayerMaskAll;
            var sqrRadius = radius * radius;
            var outputCapacity = outputArray.Length;
            var min = new Vector2(center.x - radius, center.y - radius);
            var max = new Vector2(center.x + radius, center.y + radius);

            //--- Iterate: Test all datas. ---
            var resultCount = 0;
            var count = allDatas.Count;
            for (var i = 0; i < count; i++)
            {
                var item = allDatas[i];

                //--- Test: Run the circle test. ---
                if (Test_ByCircle(item, center, sqrRadius, min, max, layerMask, checkForLayers))
                {
                    //--- Finally: Assign to current index. ---
                    outputArray[resultCount] = item.GameObject;
                    resultCount++;

                    // Check if we hit the capacity of output array.
                    if (resultCount == outputCapacity)
                    {
                        break;
                    }
                }
            }

            //--- Finally: Return result count. ---
            return resultCount;
        }

        #endregion

        #region Test Runners

        private bool Test_ByPoint(TrackedObjectData item, Vector2 point, int layerMask, bool checkForLayers)
        {
            if (checkForLayers)
            {
                //--- Step 1: Run the LayerMask test. ---
                if (!TestFor_Layer(item, layerMask))
                {
                    return false;
                }
            }

            //--- Step 2: Run the AABB test. ---
            if (!TestFor_AABB(item, point, point))
            {
                return false;
            }

            //--- Step 3: Run the PIP test. ---
            if (!TestFor_PIP(item, point))
            {
                return false;
            }

            //--- Finally: Return true (passed all tests). ---
            return true;
        }

        private bool Test_ByBox(TrackedObjectData item, Vector2 min, Vector2 max, int layerMask, bool checkForLayers)
        {
            if (checkForLayers)
            {
                //--- Step 1: Run the LayerMask test. ---
                if (!TestFor_Layer(item, layerMask))
                {
                    return false;
                }
            }

            //--- Step 2: Run the AABB test. ---
            if (!TestFor_AABB(item, min, max))
            {
                return false;
            }

            //--- Finally: Return true (passed all tests). ---
            return true;
        }

        private bool Test_ByCircle(TrackedObjectData item, Vector2 center, float sqrRadius, Vector2 aabbMin,
            Vector2 aabbMax, int layerMask, bool checkForLayers)
        {
            if (checkForLayers)
            {
                //--- Step 1: Run the LayerMask test. ---
                if (!TestFor_Layer(item, layerMask))
                {
                    return false;
                }
            }

            //--- Step 2: Run the AABB test. ---
            if (!TestFor_AABB(item, aabbMin, aabbMax))
            {
                return false;
            }

            //--- Step 3: Run the Distance test. ---
            if (!TestFor_Distance(item, center, sqrRadius))
            {
                return false;
            }

            //--- Finally: Return true (passed all tests). ---
            return true;
        }

        #endregion

        #region Tests

        private static bool TestFor_Layer(TrackedObjectData item, int layerMask)
        {
            var layerToTest = item.GameObject.layer;
            return layerMask.MaskIncludes(layerToTest);
        }

        private static bool TestFor_AABB(TrackedObjectData item, Vector2 min, Vector2 max)
        {
            var itemMin = item.AABB.Min;
            var itemMax = item.AABB.Max;
            return Geometry2D.IsOverlapping(itemMin, itemMax, min, max);
        }

        private static bool TestFor_PIP(TrackedObjectData item, Vector2 point)
        {
            IList<Vector2> hullPoints = item.WorldConvexHull;
            return point.IsInPoly(hullPoints);
        }

        private static bool TestFor_Distance(TrackedObjectData item, Vector2 center, float sqrRadius)
        {
            var testCenter = item.AABB.Center;
            return testCenter.TestDistanceLowerThan(center, sqrRadius, false);
        }

        #endregion

        #region Pickers

        private TrackedObjectData Pick_ByPoint(IList<TrackedObjectData> toPickFrom, Vector2 point, int layerMask)
        {
            var checkForLayers = layerMask != MathUtils.LayerMaskAll;
            //if layermask is 1, that means everything is included, so we don't need to check.

            var toPickFromCount = toPickFrom.Count;
            for (var i = 0; i < toPickFromCount; i++)
            {
                var item = toPickFrom[i];

                if (Test_ByPoint(item, point, layerMask, checkForLayers))
                {
                    return item;
                }
            }

            //--- Finally: Return null (nobody passed tests). ---
            return null;
        }

        private TrackedObjectData Pick_ByBox(IList<TrackedObjectData> toPickFrom, Vector2 min, Vector2 max,
            int layerMask)
        {
            var checkForLayers = layerMask != MathUtils.LayerMaskAll;
            //if layermask is 1, that means everything is included, so we don't need to check.

            var toPickFromCount = toPickFrom.Count;
            for (var i = 0; i < toPickFromCount; i++)
            {
                var item = toPickFrom[i];

                if (Test_ByBox(item, min, max, layerMask, checkForLayers))
                {
                    return item;
                }
            }

            //--- Finally: Return null (nobody passed test). ---
            return null;
        }

        private TrackedObjectData Pick_ByCircle(IList<TrackedObjectData> toPickFrom, Vector2 center, float radius,
            int layerMask)
        {
            var checkForLayers = layerMask != MathUtils.LayerMaskAll;
            //if layermask is 1, that means everything is included, so we don't need to check.

            //--- Preparation: Determine max and min points of test area. ---
            //---              Calculate squared radius.                  ---
            var min = new Vector2(center.x - radius, center.y - radius);
            var max = new Vector2(center.x + radius, center.y + radius);
            var sqrRadius = radius * radius;

            var toPickFromCount = toPickFrom.Count;
            for (var i = 0; i < toPickFromCount; i++)
            {
                var item = toPickFrom[i];

                if (Test_ByCircle(item, center, sqrRadius, min, max, layerMask, checkForLayers))
                {
                    return item;
                }
            }

            //--- Finally: Return null (nobody passed tests). ---
            return null;
        }

        #endregion

        private void Update()
        {
            ObjectTrackerDatabase.UpdateAll();
        }

        private static GameObject GetGameObjectOrNull(TrackedObjectData data)
        {
            return data != null ? data.GameObject : null;
        }
    }
}
