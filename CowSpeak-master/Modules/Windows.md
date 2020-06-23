## <div align="center">Functions</div>
Type | Name | Parameter 0 | Parameter 1 | Parameter 2 | Parameter 3
--- | --- | --- | --- | --- | ---
boolean | CloseHandle | integer64 hObject
integer | CloseWindow | integer64 hWnd
integer64 | FindWindow | string lpClassName | string lpWindowName
integer64 | GetBaseAddress | integer processID | string moduleName
string | GetErrorCodeText | integer errorCode
integer64 | GetForegroundWindow | 
integer | GetLastError | 
integer64 | GetMainWindowHandle | integer processID
integer64 | GetModuleHandle | string lpModuleName
integer | GetProcessID | string processName
integer | GetWindowLong | integer64 hWnd | integer nIndex
string | GetWindowText | integer64 hWnd
integer64 | OpenProcess | integer processAccess | boolean bInheritHandle | integer processId
ByteArray | ReadProcessMemory | integer64 hProcess | integer64 lpBaseAddress | integer bytes
boolean | SetForegroundWindow | integer64 hWnd
integer | SetWindowLong | integer64 hWnd | integer nIndex | integer dwNewLong
boolean | TerminateProcess | integer64 hProcess | integer exitCode
## <div align="center">Definitions</div>
From | To
--- | ---
ProcessAccessFlags.All | 2035711
ProcessAccessFlags.CreateProcess | 128
ProcessAccessFlags.CreateThread | 2
ProcessAccessFlags.DuplicateHandle | 64
ProcessAccessFlags.QueryInformation | 1024
ProcessAccessFlags.QueryLimitedInformation | 4096
ProcessAccessFlags.SetInformation | 512
ProcessAccessFlags.SetQuota | 256
ProcessAccessFlags.Synchronize | 1048576
ProcessAccessFlags.Terminate | 1
ProcessAccessFlags.VirtualMemoryOperation | 8
ProcessAccessFlags.VirtualMemoryRead | 16
ProcessAccessFlags.VirtualMemoryWrite | 32
WindowLongParam.DWLP_DLGPROC | 4
WindowLongParam.DWLP_MSGRESULT | 0
WindowLongParam.DWLP_USER | 8
WindowLongParam.GWL_EXSTYLE | -20
WindowLongParam.GWL_ID | -12
WindowLongParam.GWL_STYLE | -16
WindowLongParam.GWL_USERDATA | -21
WindowLongParam.GWL_WNDPROC | -4
WindowLongParam.GWLP_HINSTANCE | -6
WindowLongParam.GWLP_HWNDPARENT | -8
WindowStyles.WS_BORDER | 8388608
WindowStyles.WS_CAPTION | 12582912
WindowStyles.WS_CHILD | 1073741824
WindowStyles.WS_CLIPCHILDREN | 33554432
WindowStyles.WS_CLIPSIBLINGS | 67108864
WindowStyles.WS_DISABLED | 134217728
WindowStyles.WS_DLGFRAME | 4194304
WindowStyles.WS_GROUP | 131072
WindowStyles.WS_HSCROLL | 1048576
WindowStyles.WS_MAXIMIZE | 16777216
WindowStyles.WS_MAXIMIZEBOX | 65536
WindowStyles.WS_MINIMIZE | 536870912
WindowStyles.WS_MINIMIZEBOX | 131072
WindowStyles.WS_OVERLAPPED | 0
WindowStyles.WS_OVERLAPPEDWINDOW | 13565952
WindowStyles.WS_POPUP | 2147483648
WindowStyles.WS_POPUPWINDOW | 2156396544
WindowStyles.WS_SIZEFRAME | 262144
WindowStyles.WS_SYSMENU | 524288
WindowStyles.WS_TABSTOP | 65536
WindowStyles.WS_VISIBLE | 268435456
WindowStyles.WS_VSCROLL | 2097152
WindowStylesEx.WS_EX_ACCEPTFILES | 16
WindowStylesEx.WS_EX_APPWINDOW | 262144
WindowStylesEx.WS_EX_CLIENTEDGE | 512
WindowStylesEx.WS_EX_COMPOSITED | 33554432
WindowStylesEx.WS_EX_CONTEXTHELP | 1024
WindowStylesEx.WS_EX_CONTROLPARENT | 65536
WindowStylesEx.WS_EX_DLGMODALFRAME | 1
WindowStylesEx.WS_EX_LAYERED | 524288
WindowStylesEx.WS_EX_LAYOUTRTL | 4194304
WindowStylesEx.WS_EX_LEFT | 0
WindowStylesEx.WS_EX_LEFTSCROLLBAR | 16384
WindowStylesEx.WS_EX_LTRREADING | 0
WindowStylesEx.WS_EX_MDICHILD | 64
WindowStylesEx.WS_EX_NOACTIVATE | 134217728
WindowStylesEx.WS_EX_NOINHERITLAYOUT | 1048576
WindowStylesEx.WS_EX_NOPARENTNOTIFY | 4
WindowStylesEx.WS_EX_NOREDIRECTIONBITMAP | 2097152
WindowStylesEx.WS_EX_OVERLAPPEDWINDOW | 768
WindowStylesEx.WS_EX_PALETTEWINDOW | 392
WindowStylesEx.WS_EX_RIGHT | 4096
WindowStylesEx.WS_EX_RIGHTSCROLLBAR | 0
WindowStylesEx.WS_EX_RTLREADING | 8192
WindowStylesEx.WS_EX_STATICEDGE | 131072
WindowStylesEx.WS_EX_TOOLWINDOW | 128
WindowStylesEx.WS_EX_TOPMOST | 8
WindowStylesEx.WS_EX_TRANSPARENT | 32
WindowStylesEx.WS_EX_WINDOWEDGE | 256

