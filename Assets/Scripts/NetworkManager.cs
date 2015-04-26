using UnityEngine;
using UnityEngine.UI;
using System;
	
public class NetworkManager : MonoBehaviour
{
	void Start()
	{
		MasterServer.ipAddress = "45.55.154.129";
		MasterServer.port = 24555;
		Network.natFacilitatorIP = "45.55.154.129";
		Network.natFacilitatorPort = 24899;
	}	


	private string filterNames = string.Empty;
	private HostData[] hostList;
	private string name = "Server Name";
	private const string typeName = "PlatformFighter";
	public Canvas Charselectmenu;

	//Controls position on screen of Main Menu Buttons (create server(1) and join server(1))
	public float guiPositionX1;
	public float guiPositionY1;
	public float guiPositionX2;
	public float guiPositionY2;

	//things relating to the server list and its scroll functionality
	private Vector2 scrollPosition = new Vector2(0f, 0f);

	//booleans that define whether 2, 3, or 4 players is checked
	private bool twoPlayers = true;
	private bool threePlayers;
	private bool fourPlayers;
	//integer that is changed when # of players checkbox is checked
	private static int maxPlayers;

	//render controls what functionality the window will have and whether the window is visible or not
	private Rect windowRect = new Rect(Screen.width * 0.2f, Screen.height * 0.1f, Screen.width * 0.6f, Screen.height * 0.8f);
	private int render;

	//Create server buttons
	public float createButtonsX;
	public float bottomButtonsY;
	public static float numPlayerY1 = Screen.height * 0.1f;
	private float numPlayerY2 = numPlayerY1 + Screen.height * 0.05f;
	private float numPlayerY3 = numPlayerY1 + Screen.height * 0.1f;
	private float textY = numPlayerY1 + Screen.height * 0.15f;
	private float hideButtonX = Screen.width - Screen.width * 0.6f;
	
	//use this to add more popup windows (buttons on the windows included).
	public void DoMyWindow(int windowID)
	{
		if (this.render == 1)
		{
			if (GUI.Button(new Rect(Screen.width * createButtonsX, Screen.height * bottomButtonsY, Screen.width / 6, Screen.height / 6), "Start Server") && (((maxPlayers == 2) || (maxPlayers == 3)) || (maxPlayers == 4)))
			{
				StartServer(this.name);
			}
			if (GUI.Button(new Rect(hideButtonX, Screen.height * bottomButtonsY, Screen.width / 6,  Screen.height / 6), "Hide Window"))
			{
				this.HideWindow();
			}
			
			this.twoPlayers = GUI.Toggle(new Rect(Screen.width * createButtonsX, numPlayerY1, Screen.width / 4, Screen.height / 12), this.twoPlayers, "2 Players");
			this.threePlayers = GUI.Toggle(new Rect(Screen.width * createButtonsX, numPlayerY2, Screen.width / 4, Screen.height / 12), this.threePlayers, "3 Players");
			this.fourPlayers = GUI.Toggle(new Rect(Screen.width * createButtonsX, numPlayerY3, Screen.width / 4, Screen.height / 12), this.fourPlayers, "4 Players");
			
			if (this.twoPlayers && (maxPlayers != 2))
			{
				this.threePlayers = false;
				this.fourPlayers = false;
				maxPlayers = 2;
			}
			else if (this.threePlayers && (maxPlayers != 3))
			{
				this.twoPlayers = false;
				this.fourPlayers = false;
				maxPlayers = 3;
			}
			else if (this.fourPlayers && (maxPlayers != 4))
			{
				this.threePlayers = false;
				this.twoPlayers = false;
				maxPlayers = 4;
			}
			this.name = GUI.TextField(new Rect(Screen.width * createButtonsX, textY, Screen.width * 0.25f, 25f), this.name, 40);
		}
		if (this.render == 2)
		{
			if (GUI.Button(new Rect(hideButtonX, Screen.height * bottomButtonsY, Screen.width / 6, Screen.height / 6), "Hide Window"))
			{
				this.HideWindow();
			}
			this.filterNames = GUI.TextField(new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.25f, 25f), this.filterNames, 40);
			this.scrollPosition = GUI.BeginScrollView(new Rect(Screen.width * 0.02f, Screen.height * 0.8f * 0.05f + 30f, 
				(Screen.width * 0.6f) * 0.96f, Screen.height * 0.8f - (Screen.height * 0.05f + 35f + (Screen.height / 6))), this.scrollPosition, new Rect(0f, 0f, (Screen.width * 0.6f) * 0.5f, Screen.height * 0.8f));
			
				if (this.hostList != null)
			{
				for (int i = 0; i < this.hostList.Length; i++)
				{
					if ((this.hostList[i].gameName.ToUpper().Contains(this.filterNames.ToUpper()) || (this.filterNames == string.Empty)) && GUI.Button(new Rect(20f, (float) (20 + (110 * i)), 380f, 75f), this.hostList[i].gameName))
					{
						this.JoinServer(this.hostList[i]);
					}
				}
			}
		}
	}
	
	private void OnGUI()
	{
		if (this.render == 1)
		{
			this.windowRect = GUI.Window(0, this.windowRect, new GUI.WindowFunction(this.DoMyWindow), "Server Creation");
		}
		if (this.render == 2)
		{
			this.windowRect = GUI.Window(0, this.windowRect, new GUI.WindowFunction(this.DoMyWindow), "Server List");
		}
		if ((!Network.isClient && !Network.isServer) && (this.render == 0))
		{
			if (GUI.Button(new Rect(Screen.width * this.guiPositionX1 - Screen.width / 12, Screen.height * this.guiPositionY1 - Screen.height / 12, Screen.width / 6, Screen.height / 6), "Create Server"))
			{
				this.ShowWindow(1);
			}
			if (GUI.Button(new Rect(Screen.width * this.guiPositionX2 - Screen.width / 12, Screen.height * this.guiPositionY2 - Screen.height / 12, Screen.width / 6, Screen.height / 6), "Join Server"))
			{
				this.ShowWindow(2);
			}
			this.RefreshHostList();
		}
	}

	private void HideWindow()
	{
		this.render = 0;
	}
	
	private void JoinServer(HostData hostData)
	{
		Application.LoadLevel("Character_select_scene");
		Network.Connect(hostData);
	}

	
	private void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
		{
			this.hostList = MasterServer.PollHostList();
		}
	}
	
	private void RefreshHostList ()
	{
		MasterServer.RequestHostList("PlatformFighter");
	}
	
	private void ShowWindow(int i)
	{
		this.render = i;
	}
	

	private static void StartServer(string gameName)
	{
		Application.LoadLevel("Character_select_scene");
		Network.InitializeServer(maxPlayers, 0x61a8, !Network.HavePublicAddress());
		MasterServer.RegisterHost("PlatformFighter", gameName);
	}

	private void OnConnectedToServer()
	{
		this.gotocharselect();
	}

	public void gotocharselect() {
		Charselectmenu.enabled = true;
	}


}

