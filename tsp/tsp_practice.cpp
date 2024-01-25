#pragma warning(disable:4996)
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <string.h>

using namespace std;

const int N_GEN = 256; // 유전자수
const int N_CITIES = 30; // 도시수

const int SWAP = 300; // 초기 집단 생성시 순서를 섞기 위한 반복 횟수
const int CANDID = 10; // 토너먼트 선택시 후보 개수

const int MUTATE1 = 35; // 돌연변이 확률

const int MUTATE2 = 10; // 돌연변이 확률
const int MUTATE3 = 30; // 돌연변이 확률
const int MUTATE4 = 55; // 돌연변이 확률


const int ITER = 20000; // 최대 진화 세대수

struct Point {
	int x, y;
};

struct GeneType { // 하나의 유전자 타입
	int gene[N_CITIES];
	double fit;	// 전체 도시의 거리의 함.
};

Point Cities[N_CITIES];	// 파일에서 읽어들인 도시별 좌표값 저장
GeneType Genes[2][N_GEN];	//0, 최초세대 / 1, 엘리트 2개 복사 후 126개를 복사 후 저장
Point Start = { -380, -350 };

int Generation = 0; // 진화세대수 카운트 변수


// 파일에서 기본 데이터를 입력하는 함수
void readData() {
	char filename[128];
	FILE* in;
	//int sum = 0;

	sprintf(filename, "cities%d.txt", N_CITIES);

	in = fopen(filename, "r");

	for (int i = 0; i < N_CITIES; i++) {
		fscanf(in, "%d %d", &Cities[i].x, &Cities[i].y);
	}

	fclose(in);

	return;
}


// 파일에 최종 결과를 출력하는 함수
void writeResult(int gen, int bestindex) {
	char filename[128];
	FILE* out;
	//int sum = 0;
	sprintf(filename, "./result%d.txt", N_CITIES);
	out = fopen(filename, "w");
	///// 여기에 최종 엘리트 방문 순서를 파일로 출력하는 명령어 작성

	for (int i = 0; i < N_CITIES; i++) { // 최종 베스트 콘솔에 출력
		fprintf(out, "%d %d\n", Cities[Genes[gen][bestindex].gene[i]].x,
			Cities[Genes[gen][bestindex].gene[i]].y);
	}

	fclose(out);

	return;
}

// 랜덤으로 초기 유전자 집합을 생성하는 함수
void makeInitGenes() {
	int temp, pos1, pos2;

	srand((unsigned int)time(NULL)); // 랜덤 시드 초기화

	for (int i = 0; i < N_GEN; i++) { // 유전자 갯수 만큼 반복
		for (int k = 0; k < N_CITIES; k++) {
			Genes[0][i].gene[k] = k;
		}
		for (int j = 0; j < SWAP; j++) { // 도시 순서를 랜덤으로 섞음

			pos1 = (rand() % N_CITIES);
			pos2 = (rand() % N_CITIES);

			temp = Genes[0][i].gene[pos1];
			Genes[0][i].gene[pos1] = Genes[0][i].gene[pos2];
			Genes[0][i].gene[pos2] = temp;
			
			///// 여기에 랜덤으로 2개의 인덱스를 만들어 유전자 요소를 교환하는 코드 작성
		}
	}

	 //확인용 출력 코드
	//for (int i = 0; i < N_GEN; i++) {
	//	cout << i << ":[";
	//	for (int k = 0; k < N_CITIES; k++) {
	//		cout << Genes[0][i].gene[k] << " ";
	//	}
	//	cout << "], " << Genes[0][i].fit << "\n";
	//}
	//cout << "\n";

	return;
}

//적응도 계산 함수
void calcFitness(int gen) {
	double best = 10e10, worst = -10e10, total = 0.0, xval, yval;
	int limit;
	for (int i = 0; i < N_GEN; i++) {
		// 시작 지점과 첫 방문 도시간의 거리
		xval = (Start.x - Cities[Genes[gen][i].gene[0]].x) 
				* (Start.x - Cities[Genes[gen][i].gene[0]].x);
		yval = (Start.y - Cities[Genes[gen][i].gene[0]].y) 
				* (Start.y - Cities[Genes[gen][i].gene[0]].y);

		total = sqrt(xval + yval);

		limit = N_CITIES - 1;
		for (int k = 0; k < limit; k++) {
			// 각 도시들간의 방문 거리	// Cites :- 도시번호는 gene이 가지고 있음
			// 
			///// k번쨰 도시와 k+1번째 도시간의 거리를 위한 xval, yval을 
			///// k를 변경시켜 가며 구함
			xval = (Cities[Genes[gen][i].gene[k]].x - Cities[Genes[gen][i].gene[k + 1]].x)* (Cities[Genes[gen][i].gene[k]].x-Cities[Genes[gen][i].gene[k + 1]].x);
			yval = (Cities[Genes[gen][i].gene[k]].y - Cities[Genes[gen][i].gene[k + 1]].y) * (Cities[Genes[gen][i].gene[k]].y - Cities[Genes[gen][i].gene[k + 1]].y);

			total += sqrt(xval + yval);
		}
		// 마지막 도시와 시작점 간의 거리
		xval = (Cities[Genes[gen][i].gene[limit]].x - Start.x)
			* (Cities[Genes[gen][i].gene[limit]].x - Start.x);
		yval = (Cities[Genes[gen][i].gene[limit]].y - Start.y)
			* (Cities[Genes[gen][i].gene[limit]].y - Start.y);

		total += sqrt(xval + yval);

		Genes[gen][i].fit = total; // 적응도 저장

		if (best > total) best = total; // 확인 출력용
		if (worst < total) worst = total; // 확인 출력용

	}

	//// 확인 출력용
	//cout << gen << ":" << endl;

	//for (int i = 0; i < N_GEN; i++) {
	//	cout << i << ":[";
	//	for (int k = 0; k < N_CITIES; k++) {
	//		cout << Genes[gen][i].gene[k] << " ";
	//	}
	//	cout << "], " << Genes[gen][i].fit << "\n";
	//}

	//cout << best << ", " << worst << endl;

	return;
}


// 1개의 엘리트를 자식 유전자로 2개 복사하는 함수
void elitism(int gen) {
	double best = 10e10;
	int bestindex = 0;

	for (int i = 0; i < N_GEN; i++) { // 최고 적응도 유전자를 찾음
		if (Genes[gen][i].fit < best) {
			bestindex = i;
			best = Genes[gen][i].fit;
		}
	}

	for (int k = 0; k < N_CITIES; k++) {
		// 다음 세대의 맨 마지막 2개의 요소에 복사
		Genes[(gen + 1) % 2][N_GEN - 2].gene[k] = Genes[gen][bestindex].gene[k]; // 1등을 마지막 2개로 복사 
		Genes[(gen + 1) % 2][N_GEN - 1].gene[k] = Genes[gen][bestindex].gene[k]; // 1등 
	}	// 유전자만 복사하면 됨. 적응도는 다음 반복에서 새로 계산함
	return;
}


// 순서교차 함수
// 부모 b1, b2 인덱스를 이용하여 자식 n1, n2를 만듦.
void crossOverOrder(int gen, int b1, int b2, int n1, int n2) {
	int c1, c2;
	int t1[N_CITIES], t2[N_CITIES];
	int temp;

	c1 = rand() % N_CITIES; // 바꿀 구간 결정

	do {
		c2 = rand() % N_CITIES;
	} while (c1 == c2);

	if (c1 > c2) {  // C1에서 C2까지 교환
		temp = c1;
		c1 = c2;
		c2 = temp;
	}

	//확인 출력용
	//cout << c1 << ", " << c2 << endl;

	//for (int i = 0; i < N_CITIES; i++) {
	//	printf("%3d", Genes[gen][b1].gene[i]);
	//	//cout << Genes[gen][b1].gene[i] << "\t";
	//}
	//cout << endl;
	//for (int i = 0; i < N_CITIES; i++) {
	//	printf("%3d", Genes[gen][b2].gene[i]);
	//	//cout << Genes[gen][b2].gene[i] << "\t";
	//}
	//cout << endl << endl;

	
	///// t1, t2를 -1로 초기화
	for (int i = 0; i < N_CITIES; i++) {
		t1[i] = -1;
		t2[i] = -1;
	}
	

	///// t1, t2에 원래 유전자의 c1~c2구간을 크로스로 복사 -> for문 하나만 쓰면 됨 b1->t2, b2->t1
	for (int i = c1; i < c2+1; i++) {
		t1[i] = Genes[gen][b2].gene[i];
		t2[i] = Genes[gen][b1].gene[i];
	}

	///// 뒷부분 채우기
	///// c2 다음부터 원래 유전자의 처음부터 중복이 없을 경우 순서대로 삽입 -> c2+1 index부터 마지막까지 N_CITIES까지 -> 중복 체크는 다 돌기 -> 중복이 없다면 넣기
	for (int i = c2 + 1; i < N_CITIES; i++) {
		for (int j = 0; j < N_CITIES; j++) {
			int same = 0;
			for (int k = 0; k < N_CITIES; k++) {	// t1 -> 중복된 게 있다면(same++)  break 하고 나옴-> 중복이 없다면 t1[i]에 넣고 break
				if (Genes[gen][b1].gene[j] == t1[k]) {
					same=1;
					break;
				}
			}
			if (!same) {
				t1[i] = Genes[gen][b1].gene[j];
				break;
			}
		}
	}

	for (int i = c2 + 1; i < N_CITIES; i++) {
		for (int j = 0; j < N_CITIES; j++) {
			int same = 0;
			for (int k = 0; k < N_CITIES; k++) {	
				if (Genes[gen][b2].gene[j] == t2[k]) {
					same=1;
					break;
				}
			}
			if (!same) {
				t2[i] = Genes[gen][b2].gene[j];
				break;
			}
		}
	}

	// 앞부분 채우기
	///// 처음부터 c1 앞에까지 원래 유전자의 처음부터 중복이 없을 경우 순서대로 삽입
	for (int i = 0; i < c1; i++) {
		for (int j = 0; j < N_CITIES; j++) {
			int same = 0;
			for (int k = 0; k < N_CITIES; k++) {	// t1 -> 중복된 게 있다면(same++)  break 하고 나옴-> 중복이 없다면 t1[i]에 넣고 break
				if (Genes[gen][b1].gene[j] == t1[k]) {
					same=1;
					break;
				}
			}
			if (!same) {
				t1[i] = Genes[gen][b1].gene[j];
				break;
			}
		}
	}

	for (int i = 0; i <c1; i++) {
		for (int j = 0; j < N_CITIES; j++) {
			int same = 0;
			for (int k = 0; k < N_CITIES; k++) {
				if (Genes[gen][b2].gene[j] == t2[k]) {
					same=1;
					break;
				}
			}
			if (!same) {
				t2[i] = Genes[gen][b2].gene[j];
				break;
			}
		}
	}


	// 생성된 t1, t2 유전자 배열을 n1, n2 자식으로 복사
	for (int i = 0; i < N_CITIES; i++) { 
		Genes[(gen + 1) % 2][n1].gene[i] = t1[i];
		Genes[(gen + 1) % 2][n2].gene[i] = t2[i];
	}

	/*확인 출력용*/
	//for (int i = 0; i < N_CITIES; i++) {
	//	printf("%3d", Genes[(gen + 1) % 2][n1].gene[i]);
	//	cout << Genes[(gen + 1) % 2][n2].gene[i] << "\t";
	//}
	//cout << endl;


	//확인 출력용
	//for (int i = 0; i < N_CITIES; i++) {
	//	printf("%3d", Genes[(gen + 1) % 2][n2].gene[i]);
	//	cout << Genes[(gen + 1) % 2][n2].gene[i] << "\t";
	//}
	//cout << endl;

	//getchar();

	/*return;*/
}

//돌연변이 함수
void mutate(int gen) {
	int m1, m2;
	int p1, p2, temp;

	for (int i = 0; i < N_GEN - 2; i++) { // 마지막 2개는 엘리트이므로 건드리지 않음
		m1 = rand() % 100;	// ->
		if (m1 < MUTATE1) {// 35이하면 35% 안에 듦
			p1 = rand() % N_CITIES;
			p2 = rand() % N_CITIES;

			temp = Genes[gen][i].gene[p1];
			Genes[gen][i].gene[p1] = Genes[gen][i].gene[p2];
			Genes[gen][i].gene[p2] = temp;
			// 랜덤값이 확률범위에 들어올 경우 i번째 유전자의 임의 위치를 랜덤으로 정해
			// 두 도시번호 값을 교환

		}

	}

	return;
}

// 부모 선택 후 교차 함수 호출로 교차 실행
// 토너먼트 선택 사용
void selectTourAndCrossover(int gen) { 
	int limit = N_GEN - 2; // 엘리트 제외하고 생성하기 위함
	int g1, best1=0, best2=0;
	double bestfit;

	elitism(gen); // 엘리티즘 실행(엘리트 2개 복사)

	for (int i = 0; i < limit; i+=2) { // 2개를 뺀 나머지 자식을 생성
		
		bestfit = 10e10;
		for (int k = 0; k < CANDID; k++) { // 토너먼트 선택에 의한 첫번째 부모 선택
			// rand%N_Gen를 사용하여 g1에 저장 -> Gene[gen][g1].fit -> bset1 = g1;


			///// CADID 갯수만큼 랜덤으로 유전자를 골라 적응도값이 가장 좋은 것의 인덱스를 best1에 저장
			g1 = rand() % N_GEN;
			if (bestfit > Genes[gen][g1].fit) {
				bestfit = Genes[gen][g1].fit;
				best1 = g1;
			}

		}

		// 확인 출력용
		//cout << i << ": " << best1 << ", " << bestfit << " <-> ";
		bestfit = 10e10;
		for (int k = 0; k < CANDID; k++) {// 토너먼트 선택에 의한 두번째 부모 선택

			///// CADID 갯수만큼 랜덤으로 유전자를 골라 적응도값이 가장 좋은 것의 인덱스를 best2에 저장	
			g1 = rand() % N_GEN;
			if (bestfit > Genes[gen][g1].fit) {
				bestfit = Genes[gen][g1].fit;
				best2 = g1;
			}
		}

		if (best1 == best2) { //혹시 부모로 같은 유전자가 선택되면 하나를 바꿈
			best2 = rand() % N_GEN;
		}

		// 확인 출력용
		//cout << best2 << ", " << bestfit << endl;
		crossOverOrder(gen, best1, best2, i, i + 1); // 순서교차
	}

	// 확인 출력용
	int plus = 0;
	//for (int i = 0; i < N_GEN; i++) {
	//	if (count[i] > 0) plus++;
	//	//cout << i << ":" << count[i] << endl;
	//}

	//cout << plus << endl;

	return;
}

// 최고 적응도 유전자를 찾고 그 인덱스를 리턴
int findBestGene(int gen) {
	int bi;
	double bfit;

	bfit = Genes[gen][0].fit;
	bi = 0;

	for (int i = 1; i < N_GEN; i++) {
		if (bfit > Genes[gen][i].fit) {
			bfit = Genes[gen][i].fit;
			bi = i;
		}
	}
	return bi;
}


int main() {
	int gen = 0, bestindex;
	double bbest = 10e10, abest = 10e10;

	readData(); // 도시 좌표 데이터 입력

	makeInitGenes();	// -초기 집단을 만듦(0번 인덱스)

	while (1) {
		calcFitness(gen); // 적응도 계산

		bestindex = findBestGene(gen); // 최고적응도의 유전자 인덱스 찾아줌
		abest = Genes[gen][bestindex].fit; // 새로운 엘리트

		if (abest < bbest or Generation % 500 == 0) { // 이전 최고보다 더 좋은 것이 발견될 경우
			printf("%d: %.2lf\n", Generation, abest);
			bbest = abest;
			/*for (int i = 0; i < N_CITIES; i++) {
				printf("%d->", Genes[gen][bestindex].gene[i]);
			}
			printf("\n");*/
		}
		//cout << endl;
		if (Generation == ITER) break; // 지정 진화세대가 되면 반복 종료

		selectTourAndCrossover(gen); 
		//// 선택, 교차 실행. 0 인덱스 세대면 1 인덱스 세대에 생성
		//// 1 인덱스 세대면 0 인덱스 세대에 생성

		gen = (gen + 1) % 2; // 이 이후는 다음 세대에 대해 실행

		mutate(gen); // 돌연변이 실행

		Generation++;
		//break;
		//getchar(); // 매 세대 확인용
	}

	writeResult(gen, bestindex); // 최종 베스트 결과 파일로 출력

	//printf("\n최종 베스트 방문 순서\n");
	//for (int i = 0; i < N_CITIES; i++) { // 최종 베스트 콘솔에 출력
	//	printf("%d %d\n", Cities[Genes[gen][bestindex].gene[i]].x,
	//						Cities[Genes[gen][bestindex].gene[i]].y);
	//}
	//printf("\n");


	// 최종 베스트 출력. 확인용
	/*for (int i = 0; i < N_GEN; i++) {
		printf("%.2lf:", Genes[gen][i].fit);
		for (int k = 0; k < N_CITIES; k++) {
			printf("%d->", Genes[flip][i].gene[k]);
		}
		printf("\n");
	}
	printf("\n");*/

	return 0;
}
