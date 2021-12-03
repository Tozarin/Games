#include <stdio.h>

int main()
{
	char* word = (char*)malloc(12 * sizeof(char));
	int count = 0;

	for (char k0 = 0; k0 < 4; k0++)
	{
		word[0] = k0;
		for (char k1 = 0; k1 < 4; k1++)
		{
			word[1] = k1;
			for (char k2 = 0; k2 < 4; k2++)
			{
				word[2] = k2;
				for (char k3 = 0; k3 < 4; k3++)
				{
					word[3] = k3;
					for (char k4 = 0; k4 < 4; k4++)
					{
						word[4] = k4;
						for (char k5 = 0; k5 < 4; k5++)
						{
							word[5] = k5;
							for (char k6 = 0; k6 < 4; k6++)
							{
								word[6] = k6;
								for (char k7 = 0; k7 < 4; k7++)
								{
									word[7] = k7;
									for (char k8 = 0; k8 < 4; k8++)
									{
										word[8] = k8;
										for (char k9 = 0; k9 < 4; k9++)
										{
											word[9] = k9;
											for (char k10 = 0; k10 < 4; k10++)
											{
												word[10] = k10;
												for (char k11 = 0; k11 < 4; k11++)
												{
													word[11] = k11;
													for (int i = 0; i < 10; i++)
													{
														if (!(word[i + 1]) && !(word[i + 2])) // 0 0
														{
															if (!(word[i]) || (word[i] == 1) || (word[i] == 2)) // ќдин из трЄх стоп-кадонов (0 0 0, 1 0 0, 2 0 0)
															{
																count++;
																break;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	printf("%d", count);


	return 0;
}