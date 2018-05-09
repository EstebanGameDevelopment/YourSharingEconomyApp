using UnityEngine;
using System.Collections;
using System.IO;

/******************************************
 * 
 * SystemNavigator
 * 
 * GUI Class that will display the filesystem for the user to navigate to the files.
 * 
 * @author Esteban Gallardo
 */
public class SystemNavigator : MonoBehaviour
{
    // ----------------------------------------------
    // EVENTS
    // ----------------------------------------------	
    public const string EVENT_SYSTEMNAVIGATOR_RESET_BUTTON = "EVENT_SYSTEMNAVIGATOR_RESET_BUTTON";

    // ----------------------------------------------
    // PUBLIC MEMBERS
    // ----------------------------------------------	
    public GUISkin GuiSkin;
    public Texture2D SelectIcon;
    public Texture2D CancelIcon;
    public Texture2D FileIcon;
    public Texture2D FolderIcon;
    public Texture2D BackIcon;
    public Texture2D DriveIcon;
    public Color SelectedColor = new Color(0.6f,0.6f,0.65f);
	
	public string SearchPattern = "*.jpg|*.png|*.jpeg";

    // ----------------------------------------------
    // PRIVATE MEMBERS
    // ----------------------------------------------	
    private FileInfo m_outputFile;

	private Vector2 m_fileScroll=Vector2.zero;
    private Color m_defaultColor;
    private Rect m_guiSize;
    private GUISkin m_previousSkin;

    private DirectoryInfo m_currentDirectory;
    private FileInformation[] m_files;
    private FolderInformation[] m_folders;
    private FolderInformation[]  m_drives;
    private FolderInformation m_parentFolder;
    private bool m_getFiles = true;
    private bool m_showDrives = false;
    private int m_selectedFile = -1;

    private Vector2 m_anchor;
    private Vector2 m_anchorFileScroll;
    private bool m_isPressed = false;
    private bool m_hasBeenMoved = false;

    // -------------------------------------------
    /* 
	 * Constructor
	 */
    void Start()
    {
        string finalDirectory = ImagesController.Instance.PathLastFileBrowsed;
        if ((finalDirectory == null) || (finalDirectory.Length == 0))
        {
            finalDirectory = Directory.GetCurrentDirectory();
        }
        m_fileScroll = new Vector2(0, ImagesController.Instance.LastScrollPosition);
        m_currentDirectory = new DirectoryInfo(finalDirectory);
        m_guiSize = new Rect(0, 0, Screen.width, Screen.height);

        BasicEventController.Instance.BasicEvent += new BasicEventHandler(OnBasicEvent);
    }

    // -------------------------------------------
    /* 
	 * Destroy
	 */
    public void Destroy()
    {
        BasicEventController.Instance.BasicEvent -= OnBasicEvent;
    }

    // -------------------------------------------
    /* 
	 * OnBasicEvent
	 */
    private void OnBasicEvent(string _nameEvent, params object[] _list)
    {
        if (_nameEvent == EVENT_SYSTEMNAVIGATOR_RESET_BUTTON)
        {
            if (m_hasBeenMoved)
            {
                m_hasBeenMoved = false;
            }            
        }
    }

    // -------------------------------------------
    /* 
	 * Draw the whole system
	 */
    void OnGUI()
    {
        DrawSystemNavigator();
    }

    // -------------------------------------------
    /*
     * OnFingerDown
     */
    private void OnFingerDown(Vector2 _position)
    {
        if  ((Screen.width - _position.x) > 100)
        {
            m_anchor = new Vector2(_position.x, _position.y);
            m_anchorFileScroll = new Vector2(m_fileScroll.x, m_fileScroll.y);
            m_isPressed = true;
        }
    }

    // -------------------------------------------
    /*
     * OnFingerMove
     */
    private void OnFingerMove(Vector2 _position)
    {
        if (m_isPressed)
        {
            Vector2 currPos = new Vector2(_position.x, _position.y);
            Vector2 dif = (currPos - m_anchor);
            dif.x = 0;
            m_fileScroll = m_anchorFileScroll + dif;
            if (Vector2.Distance(m_anchorFileScroll, m_fileScroll) > 20)
            {
                m_hasBeenMoved = true;
            }            
        }
    }

    // -------------------------------------------
    /*
     * OnFingerUp
     */
    private void OnFingerUp(Vector2 _position)
    {
        if (m_isPressed)
        {
            m_isPressed = false;
            BasicEventController.Instance.DispatchBasicEvent(EVENT_SYSTEMNAVIGATOR_RESET_BUTTON, 0.3f);
        }
    }

    // -------------------------------------------
    /*
     * Update
     */
    void Update()
    {
        if (!m_isPressed)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0))
            {
                Vector2 posDown;
                if (Input.GetMouseButtonDown(0))
                {
                    posDown = Input.mousePosition;
                }
                else
                {
                    posDown = Input.GetTouch(0).position;
                }

                OnFingerDown(posDown);
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0) || ((Input.touchCount > 0) && ((Input.GetTouch(0).phase == TouchPhase.Ended))))
            {
                Vector2 posUp;
                if (Input.GetMouseButtonUp(0))
                {
                    posUp = Input.mousePosition;
                }
                else
                {
                    posUp = Input.GetTouch(0).position;
                }

                OnFingerUp(posUp);
            }
            else
            {
                Vector2 posMoved = Input.mousePosition;
                if (!Input.GetMouseButton(0))
                {
                    if (Input.touchCount > 0) posMoved = Input.GetTouch(0).position;
                }

                OnFingerMove(posMoved);
            }
        }
    }

    // -------------------------------------------
    /*
     * DrawSystemNavigator
     */
    public bool DrawSystemNavigator()
    {
		if(m_getFiles)
        {
			GetFileList(m_currentDirectory); 
			m_getFiles=false;
		}
		if (GuiSkin)
        {
			m_previousSkin = GUI.skin;
			GUI.skin = GuiSkin;
		}
		GUILayout.BeginArea(m_guiSize);
		GUILayout.BeginVertical("box");
			
        // BEGIN DRAW LAYOUT
		m_fileScroll = GUILayout.BeginScrollView(m_fileScroll);				
		if (m_showDrives)
        {
			GUILayout.BeginHorizontal();
			foreach (FolderInformation drives in m_drives)
            {
				if (drives.Button())
                {
                    GetFileList(drives.Drive);
                }
			}
			GUILayout.EndHorizontal();
        }
        else
        {
			if (m_parentFolder.Button())
            {
                GetFileList(m_parentFolder.Drive);
            }				
		}
					
		// DRAW FILES
		foreach (FolderInformation folder in m_folders)
        {
			if (folder.Button())
            {
                GetFileList(folder.Drive);
            }
		}
		for(int file=0; file < m_files.Length; file++)
        {
			if (m_selectedFile == file)
            {
				m_defaultColor = GUI.color;
				GUI.color = SelectedColor;
			}
			if(m_files[file].Button())
            {
                if (m_hasBeenMoved)
                {
                    m_hasBeenMoved = false;
                }
                else
                {
                    m_outputFile = m_files[file].File;
                    m_selectedFile = file;
                    m_files[file].LoadSelectedImage();
                }
            }
			if (m_selectedFile==file)
            {
                GUI.color = m_defaultColor;
            }				
		}
		GUILayout.EndScrollView();

        // DRAW SELECT AND CANCEL BUTTONS
        GUIContent gcSelectFile = new GUIContent("Select", SelectIcon);
        GUIContent gcCancelOperation = new GUIContent("Cancel", CancelIcon);
        if (GUILayout.Button(gcSelectFile))
        {
            if (m_outputFile != null)
            {
                BasicEventController.Instance.DispatchBasicEvent(ScreenSystemNavigatorView.EVENT_SCREENSYSTEMNAVIGATOR_SELECTED_FILE, m_outputFile.FullName, m_fileScroll.y);
                return true;
            }
        }
		if (GUILayout.Button(gcCancelOperation))
        {
            BasicEventController.Instance.DispatchBasicEvent(ScreenSystemNavigatorView.EVENT_SCREENSYSTEMNAVIGATOR_SELECTED_FILE);
            m_outputFile = null;
			return true;
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();
		if(GuiSkin)
        {
            GUI.skin = m_previousSkin;
        }
		return false;
	}

    // -------------------------------------------
    /*
     * GetFileList
     */
    public void GetFileList(DirectoryInfo _directoryInfo)
    {
        if (m_hasBeenMoved)
        {
            m_hasBeenMoved = false;
            return;
        }

        BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_CLEAR_THUMBNAIL_CACHE);
		m_currentDirectory = _directoryInfo;

		// PARENT
		if(BackIcon)
        {
            m_parentFolder = (_directoryInfo.Parent == null) ? new FolderInformation(_directoryInfo, BackIcon) : new FolderInformation(_directoryInfo.Parent, BackIcon);
        }			
		else
        {
            m_parentFolder = (_directoryInfo.Parent == null) ? new FolderInformation(_directoryInfo) : new FolderInformation(_directoryInfo.Parent);
        }
		m_showDrives = (_directoryInfo.Parent==null);
		
		// DRIVES
		string[] drives = System.IO.Directory.GetLogicalDrives();
		m_drives = new FolderInformation[drives.Length];
		for(int v = 0; v < drives.Length; v++)
        {
			m_drives[v] = (DriveIcon==null)?new FolderInformation(new DirectoryInfo(drives[v])):new FolderInformation(new DirectoryInfo(drives[v]),DriveIcon);
		}
		
		// DIRECTORIES
		DirectoryInfo[] directories = _directoryInfo.GetDirectories();
		m_folders = new FolderInformation[directories.Length];
		for(int d = 0; d < directories.Length; d++)
        {
			if (FolderIcon)
            {
                m_folders[d] = new FolderInformation(directories[d], FolderIcon);
            }				
			else
            {
                m_folders[d] = new FolderInformation(directories[d]);
            }				
		}

        // FILES
        FileInfo[] files = ImageUtils.GetFiles(_directoryInfo, SearchPattern, SearchOption.TopDirectoryOnly);
        m_files = new FileInformation[files.Length];
		for(int f=0;f<files.Length;f++)
        {
			if (FileIcon)
            {
                m_files[f] = new FileInformation(files[f], FileIcon);
            }				
			else
            {
                m_files[f] = new FileInformation(files[f]);
            }				
		}
	}
}

// -------------------------------------------
/*
 * Class FileInformation
 */
public class FileInformation
{
	public FileInfo File;
	public GUIContent GContext;
	
	public FileInformation(FileInfo _file)
    {
		File = _file;
		GContext = new GUIContent(File.Name);
	}
	
	public FileInformation(FileInfo _file, Texture2D _image)
    {
		File = _file;
        GContext = new GUIContent(File.Name, _image);
    }
	
	public bool Button()
    {
        return GUILayout.Button(GContext);
    }
	public void Label()
    {
        GUILayout.Label(GContext);
    }
	public bool Button(GUIStyle gs)
    {
        return GUILayout.Button(GContext,gs);
    }
	public void Label(GUIStyle gs)
    {
        GUILayout.Label(GContext,gs);
    }

    public void LoadSelectedImage()
    {
        BasicEventController.Instance.DispatchBasicEvent(ImagesController.EVENT_IMAGES_LOAD_THUMBNAIL_FILE_BROWSER, GContext, File.FullName);
    }
}

// -------------------------------------------
/*
 * Class FolderInformation
 */
public class FolderInformation
{
	public DirectoryInfo Drive;
	public GUIContent gc;
	
	public FolderInformation(DirectoryInfo _drive)
    {
        gc = new GUIContent(_drive.Name);
        Drive = _drive;
	}
	
	public FolderInformation(DirectoryInfo _drive, Texture2D _icon)
    {
		gc = new GUIContent(_drive.Name,_icon);
        Drive = _drive;
    }
	
	public bool Button()
    {
        return GUILayout.Button(gc);
    }
	public void Label()
    {
        GUILayout.Label(gc);
    }
	public bool Button(GUIStyle gs)
    {
        return GUILayout.Button(gc,gs);
    }
	public void Label(GUIStyle gs)
    {
        GUILayout.Label(gc,gs);
    }
}