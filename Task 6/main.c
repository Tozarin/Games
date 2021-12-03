#include <stdio.h>

int main()
{
	// count of experiments
	int exp = 0;

	// count of successes
	int suc = 0;

	// first part
	for (int i = 2; i < 23; i += 2)
	{
		for (int j = 2; j < 23; j += 2)
		{
			exp++;
			if ((i + j) == 42)
			{
				suc++;
			}
		}
	}

	float prob = (float)suc / (float)exp;
	printf("1st probability: %f\n\n", prob);


	// second part
	exp = 0;
	suc = 0;

	for (int i = 2; i < 23; i += 2)
	{
		for (int j = 2; j < 23; j += 2)
		{
			exp++;
			if ((i + j) <= 20)
			{
				suc++;
			}
		}
	}

	prob = (float)suc / (float)exp;
	printf("2nd probability: %f\n\n", prob);


	// third part
	exp = 0;
	suc = 0;

	for (int i = 2; i < 23; i += 2)
	{
		for (int j = 2; j < 23; j += 2)
		{
			for (int k = 2; k < 23; k += 2)
			{
				exp++;
				if ((i + j + k) > 60)
				{
					suc++;
				}
			}
		}
	}

	prob = (float)suc / (float)exp;
	printf("3rd probability: %f\n\n", prob);


	// fifth part
	exp = 0;
	suc = 0;

	for (int i = 2; i < 23; i += 2)
	{
		for (int j = 2; j < 23; j += 2)
		{
			for (int k = 2; k < 23; k += 2)
			{
				for (int l = 2; l < 23; l += 2)
				{
					exp++;
					if ((i + j) == (k + l))
					{
						suc++;
					}
				}
			}
		}
	}

	prob = (float)suc / (float)exp;
	printf("5th probability: %f\n\n", prob);

	return 0;
}