using BepInEx;
using GorillaLocomotion;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilla;

namespace GorillaCheckPointMod
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        GameObject point;
        bool isPressingDown;
        Color VRRig_Color;
        Vector3 gorillaposition = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player").transform.position;

        void Start()
        {
            /* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */

            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/

            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            Destroy(point);

            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
        }

        void Update()
        {
            if (!inRoom) return;
            
            if (Keyboard.current.yKey.isPressed || ControllerInputPoller.instance.leftControllerSecondaryButton)
            {
                if (!isPressingDown)
                    point.transform.position = GorillaLocomotion.Player.Instance.headCollider.transform.position;
            }
            else if (Keyboard.current.bKey.isPressed || ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                if (!isPressingDown)
                {
                   TPMETHOD(point.transform.position);
                    
                  
                }
            }
            

            isPressingDown = Keyboard.current.yKey.isPressed || Keyboard.current.bKey.isPressed ||
                             ControllerInputPoller.instance.leftControllerSecondaryButton ||
                             ControllerInputPoller.instance.rightControllerSecondaryButton;
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            point = CreateCheckpoint();

            inRoom = true;
        }
        async void TPMETHOD(Vector3 checkpointposition)
        {
            foreach (var colider in Resources.FindObjectsOfTypeAll<MeshCollider>()) 
                colider.enabled = false;
            Player.Instance.headCollider.transform.position = point.transform.position;
            await Task.Delay(50);
            foreach (var colider in Resources.FindObjectsOfTypeAll<MeshCollider>())
                colider.enabled = true; 
        }
     static GameObject CreateCheckpoint()
        {
            var checkpoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var material = new Material(Shader.Find("GorillaTag/UberShader"))
            {
                color = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player").GetComponent<VRRig>().playerColor
            };
            checkpoint.GetComponent<Renderer>().material = material;
            checkpoint.GetComponent<BoxCollider>().enabled = false;
            checkpoint.transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);
            return checkpoint;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
        }
        
       
    }
}
