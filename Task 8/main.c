#include <stdio.h>

int main()
{
	// count of experiments
	int exp = 0;

	// count of successes
	int suc = 0;

	// count of sets with 8 elements from the set with 40 elements
	// where 37, 38, 39 - marked elements
	for (int i = 0; i < 33; i++)
	{
		for (int j = i + 1; j < 34; j++)
		{
			for (int k = j + 1; k < 35; k++)
			{
				for (int l = k + 1; l < 36; l++)
				{
					for (int c = l + 1; c < 37; c++)
					{
						for (int e = c + 1; e < 38; e++)
						{
							for (int m = e + 1; m < 39; m++)
							{
								for (int t = m + 1; t < 40; t++)
								{
									exp++;
									if ((i > 36) || (j > 36) || (k > 36) || (l > 36) || (c > 36) || (e > 36) || (m > 36) || (t > 36))
									{
										suc++;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	float prob = (float)suc / (float)exp;
	printf("prob: %f", prob);

	return 0;
}