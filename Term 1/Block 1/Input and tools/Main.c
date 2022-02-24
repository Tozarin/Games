#include <stdio.h>
#include "InputAndTools.h"

int main()
{
	int a[10] = { 100, 10, 123, 1231, 6, 7, 124, 124, 9867, 21 };
	sort_of_array(a, 10);
	for (int i = 0; i < 10; i++)
	{
		printf("%d ", a[i]);
	}
	return 0;
}