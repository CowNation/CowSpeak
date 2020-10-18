## <div align="center">Functions</div>
Type | Name | Parameter 0 | Parameter 1 | Parameter 2 | Parameter 3
--- | --- | --- | --- | --- | ---
boolean | CloseHandle | integer64 hObject
integer | CloseWindow | integer64 hWnd
integer64 | FindWindow | string lpClassName | string lpWindowName
integer | GetCurrentProcessID | 
string | GetErrorCodeText | integer errorCode
integer64 | GetForegroundWindow | 
integer | GetLastError | 
integer64 | GetMainWindowHandle | integer processID
integer64 | GetModuleBaseAddress | integer procId | string modName
integer64 | GetModuleHandle | string lpModuleName
integer | GetProcessID | string processName
IntegerArray | GetProcessIDs | string processName
integer | GetWindowLong | integer64 hWnd | integer nIndex
string | GetWindowText | integer64 hWnd
object | NaGetProcess | string processName
integer64 | OpenProcess | integer processAccess | boolean bInheritHandle | integer processId
ByteArray | ReadProcessMemory | integer64 hProcess | integer64 lpBaseAddress | integer bytes
boolean | SetForegroundWindow | integer64 hWnd
integer | SetWindowLong | integer64 hWnd | integer nIndex | integer dwNewLong
boolean | TerminateProcess | integer64 hProcess | integer exitCode
## <div align="center">Definitions</div>
From | To
--- | ---
DWLP_DLGPROC | 4
DWLP_MSGRESULT | 0
DWLP_USER | 8
GWL_EXSTYLE | -20
GWL_ID | -12
GWL_STYLE | -16
GWL_USERDATA | -21
GWL_WNDPROC | -4
GWLP_HINSTANCE | -6
GWLP_HWNDPARENT | -8
PROCESS_ALL_ACCESS | 2035711
PROCESS_CREATE_PROCESS | 128
PROCESS_CREATE_THREAD | 2
PROCESS_DUP_HANDLE | 64
PROCESS_QUERY_INFORMATION | 1024
PROCESS_QUERY_LIMITED_INFORMATION | 4096
PROCESS_SET_INFORMATION | 512
PROCESS_SET_QUOTA | 256
PROCESS_TERMINATE | 1
PROCESS_VM_OPERATION | 8
PROCESS_VM_READ | 16
PROCESS_VM_WRITE | 32
SYNCHRONIZE | 1048576
WS_BORDER | 8388608
WS_CAPTION | 12582912
WS_CHILD | 1073741824
WS_CLIPCHILDREN | 33554432
WS_CLIPSIBLINGS | 67108864
WS_DISABLED | 134217728
WS_DLGFRAME | 4194304
WS_EX_ACCEPTFILES | 16
WS_EX_APPWINDOW | 262144
WS_EX_CLIENTEDGE | 512
WS_EX_COMPOSITED | 33554432
WS_EX_CONTEXTHELP | 1024
WS_EX_CONTROLPARENT | 65536
WS_EX_DLGMODALFRAME | 1
WS_EX_LAYERED | 524288
WS_EX_LAYOUTRTL | 4194304
WS_EX_LEFT | 0
WS_EX_LEFTSCROLLBAR | 16384
WS_EX_LTRREADING | 0
WS_EX_MDICHILD | 64
WS_EX_NOACTIVATE | 134217728
WS_EX_NOINHERITLAYOUT | 1048576
WS_EX_NOPARENTNOTIFY | 4
WS_EX_NOREDIRECTIONBITMAP | 2097152
WS_EX_OVERLAPPEDWINDOW | 768
WS_EX_PALETTEWINDOW | 392
WS_EX_RIGHT | 4096
WS_EX_RIGHTSCROLLBAR | 0
WS_EX_RTLREADING | 8192
WS_EX_STATICEDGE | 131072
WS_EX_TOOLWINDOW | 128
WS_EX_TOPMOST | 8
WS_EX_TRANSPARENT | 32
WS_EX_WINDOWEDGE | 256
WS_GROUP | 131072
WS_HSCROLL | 1048576
WS_MAXIMIZE | 16777216
WS_MAXIMIZEBOX | 65536
WS_MINIMIZE | 536870912
WS_MINIMIZEBOX | 131072
WS_OVERLAPPED | 0
WS_OVERLAPPEDWINDOW | 13565952
WS_POPUP | 2147483648
WS_POPUPWINDOW | 2156396544
WS_SIZEFRAME | 262144
WS_SYSMENU | 524288
WS_TABSTOP | 65536
WS_VISIBLE | 268435456
WS_VSCROLL | 2097152

