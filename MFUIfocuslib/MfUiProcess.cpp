#include "stdafx.h"
#include "opencv2/imgcodecs.hpp"
#include "opencv2/highgui.hpp"
#include "opencv2/imgproc.hpp"
#include <stdio.h>
#include <stdlib.h>

using namespace cv;
using namespace std;

int r = 255;
int g = 255;
int b = 255;

bool CheckPixel(cv::Mat img, int x, int y, bool rgb,int bgr[])
{
	if (rgb)
	{
		return img.at<cv::Vec3b>(y, x)[0] == bgr[0] && img.at<cv::Vec3b>(y, x)[1] == bgr[1] && img.at<cv::Vec3b>(y, x)[2] == bgr[2];
	}
	return img.at<cv::Vec4b>(y, x)[0] == bgr[0] && img.at<cv::Vec4b>(y, x)[1] == bgr[1] && img.at<cv::Vec4b>(y, x)[2] == bgr[2];
}

void ProcessPicForOcr(cv::Mat *img)
{
	int channels = img->channels();
	for (int i = 0; i < img->cols; i++)
	{
		for (int j = 0; j < img->rows; j++)
		{
			bool convert = false;

			convert = channels == 3 ? img->at<cv::Vec3b>(j, i)[0] != b && img->at<cv::Vec3b>(j, i)[1] != g && img->at<cv::Vec3b>(j, i)[2] != r : img->at<cv::Vec4b>(j, i)[0] != b && img->at<cv::Vec4b>(j, i)[1] != g && img->at<cv::Vec4b>(j, i)[2] != r;

			if (convert)
			{
				bool white = false;
				if (i + 1 <= img->cols)
				{
					white = channels == 3 ? img->at<cv::Vec3b>(j, i+1)[0] == b && img->at<cv::Vec3b>(j, i+1)[1] == g && img->at<cv::Vec3b>(j, i+1)[2] == r : img->at<cv::Vec4b>(j, i+1)[0] == b && img->at<cv::Vec4b>(j, i+1)[1] == g && img->at<cv::Vec4b>(j, i+1)[2]== r;
				}
				if (!white && i - 1 >= 0)
				{
					white = channels == 3 ? img->at<cv::Vec3b>(j, i - 1)[0] == b && img->at<cv::Vec3b>(j, i - 1)[1] == g && img->at<cv::Vec3b>(j, i - 1)[2] == r : img->at<cv::Vec4b>(j, i - 1)[0] == b && img->at<cv::Vec4b>(j, i - 1)[1] == g && img->at<cv::Vec4b>(j, i - 1)[2] == r;
				}
				if (!white && j + 1 <= img->rows)
				{
					white = channels == 3 ? img->at<cv::Vec3b>(j + 1, i)[0] == b && img->at<cv::Vec3b>(j + 1, i)[1] == g && img->at<cv::Vec3b>(j + 1, i)[2] == r : img->at<cv::Vec4b>(j + 1, i)[0] == b && img->at<cv::Vec4b>(j + 1, i)[1] == g && img->at<cv::Vec4b>(j + 1, i)[2] == r;
				}
				if (!white && j - 1 >= 0)
				{
					white = channels == 3 ? img->at<cv::Vec3b>(j - 1, i)[0] == b && img->at<cv::Vec3b>(j - 1, i)[1] == g && img->at<cv::Vec3b>(j - 1, i)[2] == r : img->at<cv::Vec4b>(j - 1, i)[0] == b && img->at<cv::Vec4b>(j - 1, i)[1] == g && img->at<cv::Vec4b>(j - 1, i)[2] == r;
				}
				convert = !white;
			}

			if (convert)
			{
				if (channels == 3)
				{
					img->at<cv::Vec3b>(j, i)[0] = 0;
					img->at<cv::Vec3b>(j, i)[1] = 0;
					img->at<cv::Vec3b>(j, i)[2] = 0;
					continue;
				}
				img->at<cv::Vec4b>(j, i)[0] = 0;
				img->at<cv::Vec4b>(j, i)[1] = 0;
				img->at<cv::Vec4b>(j, i)[2] = 0;
			}
		}
	}
	
}

bool predite(cv::Mat img, bool lineItem, int x1, int y1, int *x2, int *y2,bool rgb, int bgr[])
{
	int vLeftLength = 0;
	int vRightLength = 0;

	int hUpLength = 0;
	int hDownLength = 0;

	int i = x1;

	for (int j = y1 + 1; j < img.rows; j++)
	{
		if (CheckPixel(img, i, j, rgb, bgr))
		{
			hUpLength++;
			continue;

		}
		*y2 = j - 1;
		break;
	}

	int j = y1;
	for (int i = x1 + 1; i < img.cols; i++)
	{
		if (CheckPixel(img, i, j, rgb, bgr))
		{
			vLeftLength++;
			continue;

		}
		*x2 = i - 1;
		break;
	}

	//middle checkpont

	if (!CheckPixel(img, *x2, *y2, rgb, bgr))
	{
		return false;
	}

	i = *x2;
	for (int j = y1 + 1; j <= *y2; j++)
	{
		if (CheckPixel(img, i, j, rgb, bgr))
		{
			hDownLength++;
			continue;

		}
		break;
	}

	j = *y2;
	for (int i = x1 + 1; i <= *x2; i++)
	{
		if (CheckPixel(img, i, j, rgb, bgr))
		{
			vRightLength++;
			continue;

		}
		break;
	}
	if (lineItem)
	{
		return hUpLength == hDownLength && vLeftLength == vRightLength && abs(vLeftLength - vRightLength) < 5;
	}
	return hUpLength == hDownLength && vLeftLength == vRightLength;
}

void parseimg(char* inputImagePath, char* outimagepath, bool lineItem, int lineItemWide, int bgr[],bool changeBackground)
{
	//printf("Debug - srcPath: %s, outpath: %s, lineItem: %d, b=%d, g=%d, r=%d", inputImagePath, outimagepath, lineItem, focusbgr[0], focusbgr[1], focusbgr[2]);
	cv::Mat img;
	img = imread(inputImagePath, IMREAD_UNCHANGED);
	
	int x1 = 0;
	int y1 = 0;
	int x2 = 0;
	int y2 = 0;

	bool found = false;
	//printf("Debug - orgImg info: data: %s", *img.data);
	bool rgb = img.channels() == 3 ? true : false;
	//printf("Debug - rgb: %d, CH=%d", rgb,img.channels());
	for (int i = 0; i < img.cols; i++)
	{
		bool stop = false;
		for (int j = 0; j < img.rows; j++)
		{
			if (CheckPixel(img, i, j, rgb, bgr))
			{
				if (predite(img, lineItem, i, j, &x2, &y2, rgb, bgr))
				{
					x1 = i;
					y1 = j;
					stop = true;
					found = true;
					break;
				}
			}
		}
		if (stop)
			break;
	}
	//printf("Debug - find it: %d", found);
	if (found)
	{
		Mat subImage;
		if (lineItem)
		{
			int deltay = y1 - lineItemWide > 0 ? lineItemWide : y1;
			subImage = img(Range(y1 - deltay, y1), Range(x1, x2));
		}
		else
			subImage = img(Range(y1, y2), Range(x1, x2));
		if (changeBackground)
		{
			ProcessPicForOcr(&subImage);
		}
		imwrite(outimagepath, subImage);
		subImage.release();
	}
	//else
	//{
	//	printf("Can not find focus\r\n");
	//}
	img.release();
}

bool ComparePicturesProcess(char* pic1path, char* pic2path)
{
	cv::Mat img1;
	cv::Mat img2;
	cv::Mat diff;

	img1 = imread(pic1path, IMREAD_UNCHANGED);
	img2 = imread(pic2path, IMREAD_UNCHANGED);

	int ch1 = img1.channels();
	int ch2 = img2.channels();

	if (img1.rows != img2.rows || img1.cols != img2.cols || ch1 != ch2)
	{
		return false;
	}


	bool rgb = false;

	if (ch1 == 3)
	{
		rgb = true;
	}

	int rows = img1.rows;
	int cols = img1.cols;

	cv::compare(img1, img2, diff, CMP_EQ);

	img1.release();
	img2.release();
	int bgr[] = { 255, 255, 255 };
	for (int i = 0; i < cols; i++)
	{
		for (int j = 0; j < rows; j++)
		{
			if (!CheckPixel(diff, i, j, rgb, bgr))
			{
				diff.release();
				return false;
			}
		}
	}
	diff.release();
	return true;
}
