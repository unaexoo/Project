#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <iostream>
#include <cstring>
#include <winsock2.h>
#include <vector>
#include <thread>
#include <string>
#include <ctime>
#include <Windows.h>
#include <algorithm>
#include <mutex>
#include <conio.h>
#include <mmsystem.h>
#pragma comment(lib, "winmm.lib")
#pragma comment(lib, "ws2_32.lib")

#define MAGIC_KEY 224
#define SPACE 32

using namespace std;
vector<int> black = { 0,2,4,6,8 };
vector<int> white = { 1,3,5,7 };

vector<int>use_black = {};
vector<int> use_white = {};
struct Setting {
    int turn_start; // 누구 턴
    vector<int> black_card;
    vector<int> white_card;
};

void receiveMsg(SOCKET clientSock) {
    char buffer[1024];
    int received = recv(clientSock, buffer, sizeof(buffer), 0);
    if (received == SOCKET_ERROR || received == 0) {
        cerr << "Server disconnected" << endl;
        closesocket(clientSock);
        exit(1);
    }

    buffer[received] = '\0';

    // 서버로부터 받은 데이터를 출력하거나 처리하는 로직을 추가
    cout << buffer << endl;
}

struct Client {
    SOCKET clientSock;
    int playerNum;
    void sendMsg(const char* msg) {
        send(clientSock, msg, strlen(msg), 0);
    }
    string strReceiveMsg() {
        char buffer[1024];
        int received = recv(clientSock, buffer, sizeof(buffer), 0);
        if (received == SOCKET_ERROR || received == 0) {
            cerr << "Server disconnected" << endl;
            closesocket(clientSock);
            exit(1);
        }
        buffer[received] = '\0';
        return string(buffer);
    }
};

enum MENU
{
    GAMESTART = 0,
    INFO,
    QUIT
};

enum KEYBOARD
{
    UP = 72,
    LEFT = 75,
    RIGHT = 77,
    DOWN = 80
};


void gotoxy(int, int);
void SetConsoleView();
void ViewUserCursor(int&);
MENU ReadyGame();
void InfoGame();
void ViewReadyGame();
void ViewInfoGame();
void sound();
void gameStart();
void ViewWinner(const int);

//void Menu();
void Basic_Setting(Setting&);
void SetConsoleView();
void ViewInfoGame();
void ViewReadyGame();
//void Distribution(vector<int>& bl, vector<int>& wh);
void my_card(vector<int>, vector<int>);
void white_card(vector<int>);
void black_card(vector<int>);
//int play_turn();
//int game(Setting& set);

void my_card(vector<int> , vector<int> );

void handleServer(Client client, Setting& set);
void playGame(Client client, Setting& set);
int client_game(Setting& set);
void sendMsg(SOCKET clientSock, const string& msg) {
    send(clientSock, msg.c_str(), msg.size(), 0);
}


Client client;
int main() {
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "Failed to initialize Winsock" << endl;
        return 1;
    }

    SOCKET clientSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (clientSocket == INVALID_SOCKET) {
        cerr << "Failed to create client socket" << endl;
        WSACleanup();
        return 1;
    }

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(12345); // 서버 포트 번호
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1"); // 서버 IP 주소

    if (connect(clientSocket, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        cerr << "Failed to connect to the server" << endl;
        closesocket(clientSocket);
        WSACleanup();
        return 1;
    }

    cout << "Connected to the server" << endl;

    client.clientSock= clientSocket;

    Setting set;
    set.turn_start = 0; // 초기 턴 설정

    // 클라이언트는 서버로부터 메시지를 기다리고, 받은 메시지에 따라 동작합니다.
    while (true) {
        handleServer(client, set);
    }
    mciSendString(TEXT("bgm.wav"), NULL, 0, NULL);

    closesocket(clientSocket);
    WSACleanup();

    return 0;
}

void handleServer(Client client, Setting& set) {

    string msg = client.strReceiveMsg();
    //int turn = stoi(msg.substr(0, msg.find(" ")));

    char buffer[1024];
    if (msg.find("TURNSTART") != string::npos) {
        int recv_turn = recv(client.clientSock, buffer, sizeof(buffer), 0);
        if (recv_turn == SOCKET_ERROR || recv_turn == 0) {
            cerr << "Server disconnected" << endl;
            closesocket(client.clientSock);
            exit(1);
        }
        buffer[recv_turn] = '\0';
        int turn = stoi(buffer + 10); // "TURNSTART " 접두사를 건너뛰기
        cout << "전달받은 turn_start 값: " << turn << endl;
        set.turn_start = turn;
    }
    if (msg=="GameStart") {
        cout << "Game start!" << endl;
        playGame(client, set);
    }
}

void playGame(Client client, Setting& set) {
    switch (ReadyGame()) {
    case GAMESTART:
        client.sendMsg("GameStart");
        //sound();
        gameStart();
        break;
    case INFO:
        InfoGame();
        break;
    case QUIT:
        client.sendMsg("Quit");
        closesocket(client.clientSock);
        WSACleanup();
        exit(0);
    }
}


void gotoxy(int x, int y)
{
    COORD Pos;
    Pos.X = 2 * x;
    Pos.Y = y;
    SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), Pos);
}

void SetConsoleView()
{
    system("title Black_White");
}

void ViewUserCursor(int& y)
{
    if (y <= 0)
    {
        y = 0;
    }
    else if (y >= 2)
    {
        y = 2;
    }

    gotoxy(15, 20 + y);
    cout << ">";
}

MENU ReadyGame()
{

    int y = 0;
    int input = 0;
    while (true)
    {
        ViewReadyGame();
        ViewUserCursor(y);
        input = _getch();
        if (input == MAGIC_KEY)
        {
            switch (_getch())
            {
            case UP:
                --y;
                break;
            case DOWN:
                ++y;
                break;
            }
        }
        else if (input == SPACE)
        {
            switch (y)
            {
            case 0:
                return GAMESTART;
            case 1:
                return INFO;
            case 2:
                return QUIT;
            }
        }
    }
}

void InfoGame()
{
    ViewInfoGame();
    system("pause>null");
}

void ViewReadyGame()
{
    system("cls");
    gotoxy(5, 2);
    cout << "******************************************************";
    gotoxy(5, 3);
    cout << "*                                                    *";
    gotoxy(5, 4);
    cout << "*                                                    *";
    gotoxy(5, 5);
    cout << "*         **                                         *";
    gotoxy(5, 6);
    cout << "*      ********     *******  *     *    *  *  *      *";
    gotoxy(5, 7);
    cout << "*         **              *  *     ******  *  *      *";
    gotoxy(5, 8);
    cout << "*        ****             *  *     ******  *  *      *";
    gotoxy(5, 9);
    cout << "*       *    *        *   *  ***   *    *  ****      *";
    gotoxy(5, 10);
    cout << "*        ****      ***********     ******  *  *      *";
    gotoxy(5, 11);
    cout << "*     **********             *             *  *      *";
    gotoxy(5, 12);
    cout << "*       *******                       ********       *";
    gotoxy(5, 13);
    cout << "*             *                              *       *";
    gotoxy(5, 14);
    cout << "*             *                              *       *";
    gotoxy(5, 15);
    cout << "*                                                    *";
    gotoxy(5, 16);
    cout << "*                                                    *";
    gotoxy(5, 17);
    cout << "*                 < Black and White >                *";
    gotoxy(5, 18);
    cout << "*                                                    *";
    gotoxy(5, 19);
    cout << "*                                                    *";
    gotoxy(5, 20);
    cout << "*                      GameStart                     *";
    gotoxy(5, 21);
    cout << "*                      GameInfo                      *";
    gotoxy(5, 22);
    cout << "*                        Quit                        *";
    gotoxy(5, 23);
    cout << "*                                                    *";
    gotoxy(5, 24);
    cout << "*                                                    *";
    gotoxy(5, 25);
    cout << "*                                                    *";
    gotoxy(5, 26);
    cout << "******************************************************";
}

void ViewInfoGame()
{
    system("cls");
    cout << " "; cout << "*****************************************************************************\n";
    cout << " "; cout << "*                                                                           *\n";
    cout << " "; cout << "*  Developer : 이오공                                                       *\n";
    cout << " "; cout << "*  music     : Raon Raul - 비눗방울                                         *\n";
    cout << " "; cout << "*              Follow Artist -  https://bit.ly/3XnYnNQ                      *\n";
    cout << " "; cout << "*              Music promoted by DayDreamSound - https://bit.ly/3ZIA5zv     *\n";
    cout << " "; cout << "*                                                                           *\n";
    cout << " "; cout << "*              Thank you.                                                   *\n";
    cout << " "; cout << "*                                                                           *\n";
    cout << " "; cout << "*****************************************************************************\n\n";

}

void gameStart()
{
    PlaySound(TEXT("bgm.wav"), NULL, SND_NODEFAULT | SND_ASYNC | SND_LOOP);
    clock_t startTime, endTime;
    startTime = clock();

    system("cls");
    Sleep(1000);
    cout << " "; cout << "*************************************************\n";
    cout << " "; cout << "*             < 게임을 시작합니다.>             *\n";
    cout << " "; cout << "*************************************************\n\n";

    
    Setting set;
    set.turn_start;
    if (set.turn_start == 0)
    {
        Sleep(1000);
        cout << " "; cout << "> 선 플레이어는 Player1 입니다" << endl << endl;
    }
    else
    {
        Sleep(1000);
        cout << "> 선 플레이어는 Player2 입니다" << endl << endl;;
    }
   
    Basic_Setting(set);
;
    int win = client_game(set);

    /*
    if (win == 0) {
        cout << " "; cout << "*************************************************\n";
        cout << " "; cout << "*            << 최종 우승 Player 1 >>           *\n";
        cout << " "; cout << "*************************************************\n\n";
    }
    else {
        cout << " "; cout << "*************************************************\n";
        cout << " "; cout << "*            << 최종 우승 Player 2 >>           *\n";
        cout << " "; cout << "*************************************************\n\n";
    }
    */
}

void ViewWinner(const int playTime)
{
    Setting set;

    recv(client.clientSock, reinterpret_cast<char*>(&set.turn_start), sizeof(set.turn_start), 0);
    Basic_Setting(set);

    int win = client_game(set);

    if (win == 0) {
        gotoxy(8, 8);
        cout << " "; cout << "*************************************************\n";
        gotoxy(8, 9);
        cout << " "; cout << "*            << 최종 우승 Player 1 >>           *\n";
        gotoxy(8, 10);
        cout << " "; cout << "*               " << playTime << "sec               *\n";
        gotoxy(8, 11);
        cout << " "; cout << "*************************************************\n\n";
        system("pause>null");
    }
    else {
        gotoxy(8, 8);
        cout << " "; cout << "*************************************************\n";
        gotoxy(8, 9);
        cout << " "; cout << "*            << 최종 우승 Player 2 >>           *\n";
        gotoxy(8, 10);
        cout << " "; cout << "*               " << playTime << "sec               *\n";
        gotoxy(8, 11);
        cout << " "; cout << "*************************************************\n\n";
        system("pause>null");
    }
}

void Basic_Setting(Setting& set)
{

    recv(client.clientSock, (char*)" *************************************************\n", sizeof(" *************************************************\n"), 0);
    recv(client.clientSock, (char*)" *                 < 카드 분배>                  *\n", sizeof(" *                 < 카드 분배>                  *\n"), 0);
    recv(client.clientSock, (char*)" *************************************************\n", sizeof(" *************************************************\n"), 0);
    recv(client.clientSock, (char*)"GAMESTART", strlen("GAMESTART"), 0);

    recv(client.clientSock, (char*)"> 딜러가 카드를 분배합니다", strlen("> 딜러가 카드를 분배합니다"), 0);
   
    Sleep(1000);
    recv(client.clientSock, (char*)" 3 ", strlen(" 3 "), 0);

    Sleep(1000);
    recv(client.clientSock, (char*)" 2 ", strlen(" 2 "), 0);

    Sleep(1000);
    recv(client.clientSock, (char*)" 1 ", strlen(" 1 "), 0);

    cout << " "; cout << "> card 분배 끝" << endl << endl;
    my_card(black, white);
    cout << endl;

    //Distribution(p1_bl, p1_wh);
    my_card(set.black_card,set.white_card);

}


void my_card(vector<int>  bl, vector<int>  wh) {
    
    for (int i = 0; i < bl.size(); i++) {
        cout << "□ ";
    }

    for (int i = 0; i < wh.size(); i++) {
        cout << "■ ";
    }
    cout << endl;

    for (int i = 0; i < bl.size(); i++) {
        cout << bl[i] << " ";
    }

    for (int i = 0; i < wh.size(); i++) {
        cout << wh[i] << " ";
    }
    cout << endl;
}

void black_card(vector<int> bl) {
    cout << "----- black card -----" << endl;
    for (int i = 0; i < bl.size(); i++) {
        cout << "□ ";
    }
    cout << endl;

    for (int i = 0; i < bl.size(); i++) {
        cout << bl[i] << " ";
    }
    cout << endl;
}

void white_card(vector<int> wh) {
    cout << "----- white card -----" << endl;
    for (int i = 0; i < wh.size(); i++) {
        cout << "■ ";
    }
    cout << endl;

    for (int i = 0; i < wh.size(); i++) {
        cout << wh[i] << " ";
    }
    cout << endl;
}

//int play_turn() {
//    srand(time(0));
//    int res = 0;
//    cout << " "; cout << "> 딜러가 플레이어의 순서를 결정합니다" << endl;
//
//    Sleep(1000);
//    cout << " 3 " << endl;
//    Sleep(1000);
//    cout << " 2 " << endl;
//    Sleep(1000);
//    cout << " 1 " << endl << endl;
//
//
//    int PlayerOne = rand() % 2 + 1;
//
//    if (PlayerOne == 1)
//    {
//        Sleep(1000);
//        cout << " "; cout << "> 선 플레이어는 Player1 입니다" << endl << endl;
//        return res = 0;
//    }
//    else
//    {
//        Sleep(1000);
//        cout << "> 선 플레이어는 Player2 입니다" << endl << endl;;
//        return res = 1;
//    }
//
//    cout << endl;
//}

/*
void Distribution(vector<int>& bl, vector<int>& wh) {

    // Black 카드 분배
    cout << "Black 카드를 원하는 순서대로 섞으세요" << " ( ,로 구분해 원하는 순서로 섞어주세요) " << endl;

    bl.clear();
    while (bl.size() < 5) {
        string black_input = "";
        cin >> black_input;
        size_t pos = 0;

        while ((pos = black_input.find(',')) != string::npos) {
            bl.push_back(stoi(black_input.substr(0, pos)));
            black_input.erase(0, pos + 1);
        }
        bl.push_back(stoi(black_input));

        if (bl.size() != 5) {
            cout << "5개의 숫자를 입력하세요." << endl;
            bl.clear();
        }
    }

    cout << "Black Card 업데이트" << endl;
    black_card(bl);
    cout << "--------------------------------------" << endl<<endl;

    // White 카드 분배
    cout << "White 카드를 원하는 순서대로 섞으세요" << " ( ,로 구분해 원하는 순서로 섞어주세요) " << endl;

    wh.clear();
    while (wh.size() < 4) {
        string white_input = "";
        cin >> white_input;
        size_t pos = 0;

        while ((pos = white_input.find(',')) != string::npos) {
            wh.push_back(stoi(white_input.substr(0, pos)));
            white_input.erase(0, pos + 1);
        }
        wh.push_back(stoi(white_input));

        if (wh.size() != 4) {
            cout << "4개의 숫자를 입력하세요." << endl;
            wh.clear();
        }
    }

    cout << "White 카드 업데이트" << endl;
    white_card(wh);
    cout << endl;
}
*/
int client_game(Setting& set) { // 창에서 닫혀버림
    int round = 0;
    int p1_score = 0, p2_score = 0;
    int my_num = 0;
    int opponent_num = 0;

    while (round < 9) {
        string roundMsg = client.strReceiveMsg();
        cout << roundMsg << endl; // "Round 1 Start" 등

        if (set.turn_start == client.playerNum) {
            cout << "제출할 숫자를 입력해주세요: ";
            cin >> my_num;
            string numMsg = "NUM " + to_string(my_num);
            sendMsg(client.clientSock, numMsg.c_str());
        }
        else {
            string num_op = client.strReceiveMsg();
            num_op = stoi(num_op.substr(num_op.find_last_of(' ') + 1));
        }

        // 서버로부터  결과 받기
        string resultMsg = client.strReceiveMsg();
        cout << resultMsg << endl;

        // 점수 업데이트 
        if (resultMsg.substr(0, 16) == "Winner is Player1") {
            p1_score++;
   
        }
        else if (resultMsg.substr(0, 16) == "Winner is Player2") {
            p2_score++;
        }
        else if (resultMsg == "Draw") {
            p1_score++;
            p2_score++;
        }
        cout << "Player 1: " << p1_score << ", Player 2: " << p2_score << endl;

        round++;

        // 게임 종료 조건 체크
        if (round == 9) {
            break;
        }
    }

    // 최종 승자 메시지 
    string finalMsg = ((char*)client.clientSock);
    cout << finalMsg << endl; 

    return set.turn_start;
}
int game(Setting& set)
{
    int round = 0;
    int p1_score = 0, p2_score = 0;
    int p1_num = 0, p2_num = 0;
    clock_t startTime, endTime;
    startTime = clock();

    while (round < 9) {
        cout << " "; cout << "*-------------------------------------------------*\n";
        cout << " "; cout << "|               Player 1 현재 Card                |\n";
        cout << " "; cout << "*-------------------------------------------------*\n";
        my_card(black, white);
        cout << endl << endl;

        cout << " "; cout << "*-------------------------------------------------*\n";
        cout << " "; cout << "|               Player 2 현재 Card                |\n";
        cout << " "; cout << "*-------------------------------------------------*\n";
        my_card(black, white);
        cout << endl << endl;

        cout << " "; cout << "*-------------------------------------------------*\n";
        printf(" *                 Round %d Start                   *\n", round + 1);
        cout << " "; cout << "*-------------------------------------------------*\n";

        if (set.turn_start == 0) {

            cout << " ";  cout << "> 제출할 숫자를 입력해주세요 " << endl;
            cout << " ";  cout << "> Player 1 : ";
            cin >> p1_num;
            cout << endl;
            /*
            cout << " ";  cout << "> 선택할 수 있는 숫자: ";
            black_card(p2_bl);
            white_card(p2_wh);
            cout << endl;
            */

            cout << " "; cout << "> 제출할 숫자를 입력해주세요 " << endl;
            cout << " "; cout << "> Player 2 : ";
            cin >> p2_num;
            cout << endl;

        }
        else {
            cout << " "; cout << "> 제출할 숫자를 입력해주세요 " << endl;
            cout << " "; cout << "> Player 2 : ";
            cin >> p2_num;
            cout << endl;

            /*
            cout << " "; cout << ">선택할 수 있는 숫자:";
            black_card(p2_bl);
            white_card(p2_wh);
            cout << endl;
            */

            cout << " "; cout << "> 제출할 숫자를 입력해주세요 " << endl;
            cout << " "; cout << "> Player 1 : ";
            cin >> p1_num;
            cout << endl;
        }

        printf(" ----------------- Round %d Result -----------------\n", round + 1);
        if (p1_num > p2_num) {
            p1_score++;
            cout << " "; cout << "> Winner is Player 1 " << endl << endl;
            cout << " "; cout << "> Player 1 : Player 2 = " << p1_score << " : " << p2_score << endl << endl;
            set.turn_start = 0;
        }
        else if (p1_num < p2_num) {
            p2_score++;
            cout << " "; cout << "> Winner is Player 2  " << endl << endl;
            cout << " "; cout << "> Player 1 : Player 2 = " << p1_score << " : " << p2_score << endl << endl;
            set.turn_start = 1;
        }
        else {
            p1_score++;
            p2_score++;
            cout << " "; cout << "> Both Get Score " << "Player 1 Score +1 : " << p1_score << ", Player 2 Score + 1 : " << p2_score << endl;
            cout << " "; cout << "> Player 1 : Player 2 = " << p1_score << " : " << p2_score << endl << endl;
        }

        round++;

        endTime = clock();
        int playTime = static_cast<int>((endTime - startTime) / CLOCKS_PER_SEC);

        if (p1_score > p2_score && round == 9) {
            cout << " "; cout << "******************* CONGRATULATION! *****************\n";
            cout << " "; cout << "*                최종 우승 : Player 1               *\n";
            cout << " "; cout << "*                플레이 타임 : " << playTime << "sec                *\n";
            cout << " "; cout << "*****************************************************\n\n";
            system("pause>null");
            PlaySound(NULL, NULL, 0);
        }
        else if (p1_score < p2_score && round == 9) {
            cout << " "; cout << "******************* CONGRATULATION! *****************\n";
            cout << " "; cout << "*                최종 우승 : Player 2               *\n";
            cout << " "; cout << "*                플레이 타임 : " << playTime << "sec                *\n";
            cout << " "; cout << "*****************************************************\n\n";
            system("pause>null");
            PlaySound(NULL, NULL, 0);
        }
        //재진행
        if (p1_score == p2_score && round == 8) {
            round = 0;
            cout << " "; cout << "> 동점 발생" << endl;
            cout << " "; cout << "> 게임을 재진행합니다." << endl;
            Basic_Setting(set);
            p1_score =0;
            p2_score = 0;
        }
    }

    return set.turn_start;

}


