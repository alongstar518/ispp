// MFUIfocuslib.cpp : Defines the exported functions for the DLL application.
//
#include "stdafx.h"
#include "MfUiProcess.h"

#define DLLEXPORT __declspec(dllexport)

EXTERN_C DLLEXPORT
void WINAPI GetFocusImg(char* orgimagepath, char* outimagepath, bool lineItem, int lineItemWide, int bgr[] ,bool changeBackground)
{
	parseimg(orgimagepath, outimagepath, lineItem, lineItemWide, bgr, changeBackground);
}

EXTERN_C DLLEXPORT
bool WINAPI CompareImage(char* img1path, char* img2path)
{
	return ComparePicturesProcess(img1path, img2path);
}