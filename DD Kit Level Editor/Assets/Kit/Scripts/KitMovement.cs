using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit
{
    public class KitMovement : MonoBehaviour
    {
        public float speed = 10, slowSpeed = 5, cursorSpeed = 25;
        public Transform cursor;
        public KitLevelWindow levelEditor;
        Vector3 previousPosition = Vector3.zero, snappedPos = Vector3.zero, snappedEuler = Vector3.zero;
        float currentSpeed = 10;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.GetComponent<KitMaterialWindow>() != null)
                    {
                        Repick(hit.collider.gameObject.GetComponent<KitMaterialWindow>());
                    }
                    else if (hit.collider.gameObject.GetComponent<KitMaterialPointer>() != null)
                    {
                        Repick(hit.collider.gameObject.GetComponent<KitMaterialPointer>().matEditor);
                    }
                }
            }
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");
            transform.root.Translate((transform.right * h + Vector3.Cross(transform.right, transform.root.up) * v) * currentSpeed * Time.deltaTime, Space.World);
            if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.E))
            {
                transform.root.Translate(Vector3.up, Space.World);
            }
            else if (Input.GetKeyDown(KeyCode.PageDown) || Input.GetKeyDown(KeyCode.Q))
            {
                transform.root.Translate(Vector3.down, Space.World);
            }

            if (previousPosition != transform.root.position)
            {
                var step = 1f;
                var stepEuler = 45f;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    step = .5f;
                    stepEuler = 15f;
                    currentSpeed = slowSpeed;
                }
                else
                {
                    currentSpeed = speed;
                }
                var step1 = step;
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    step = 5f;
                    step1 = 1f;
                }
                snappedPos = new Vector3(RoundTo(transform.root.position.x, step), RoundTo(transform.root.position.y, step1), RoundTo(transform.root.position.z, step));
                levelEditor.spawnPosition = snappedPos;
                snappedEuler = new Vector3(0, RoundTo(transform.eulerAngles.y, stepEuler), 0);
                levelEditor.spawnRotation = Quaternion.Euler(snappedEuler);
            }
            cursor.position = Vector3.Lerp(cursor.position, snappedPos, Time.deltaTime * cursorSpeed);
        }

        public void Repick(KitMaterialWindow matEditor)
        {
            matEditor.enabled = true;
            if (matEditor.enabled)
            {
                KitMaterialWindow[] mws = FindObjectsOfType(typeof(KitMaterialWindow)) as KitMaterialWindow[];
                foreach (var mw in mws)
                {
                    if (mw != matEditor)
                    {
                        mw.enabled = false;
                    }
                }
            }
        }

        public static float RoundTo(float numToRound, float step)
        {
            if (step == 0)
                return numToRound;

            //Calc the floor value of numToRound
            float floor = ((int)(numToRound / step)) * step;

            //round up if more than half way of step
            float round = floor;
            float remainder = numToRound - floor;
            if (remainder >= step / 2)
                round += step;

            return round;
        }
    }
}
