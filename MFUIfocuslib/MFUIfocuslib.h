
extern "C" {

	void WINAPI GetFocusImg(char* orgimagepath, char* outimagepath, bool lineItem, int lineItemWide, int focusbgr[]);
	bool WINAPI CompareImage(char* img1path, char* img2path);

}