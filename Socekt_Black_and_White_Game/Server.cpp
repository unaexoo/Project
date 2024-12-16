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

#define CLIENT_CNT 2
#define MAX_SIZE 1024
using namespace std;

struct Setting {
    int turn_start; // 누구 턴
    mutex mutex_setting; // mutex 
};

struct Client  {
    SOCKET clientSock;
    int playerNum;
    bool isStart; // 게임 시작 여부
    string player = " ";
    vector<int> black_card;
    vector<int> white_card;
    mutex mutex_client; // mutex 
};

Client client[CLIENT_CNT];
void handle(Client& client, Setting& set);
void startGame(Client& client, Setting& set);
void sendMsg(const char* msg, int idx);
void game_handle(Client& new_client, Setting& set, int& round, int& p1_score, int& p2_score);

int play_turn() {
    srand(time(0));
    int res = 0;
    sendMsg("> 딜러가 플레이어의 순서를 결정합니다", CLIENT_CNT);
    Sleep(1000);
    sendMsg(" 3 ", CLIENT_CNT);
    Sleep(1000);
    sendMsg(" 2 ", CLIENT_CNT);
    Sleep(1000);
    sendMsg(" 1 ", CLIENT_CNT);


    int PlayerOne = rand() % 2 + 1;

    if (PlayerOne == 1)
    {
        Sleep(1000);
        sendMsg(" > 선 플레이어는 Player1 입니다", CLIENT_CNT);
        res = 0;
    }
    else
    {
        Sleep(1000);
        sendMsg(" > 선 플레이어는 Player2 입니다", CLIENT_CNT);
        res = 1;
    }

    cout << endl;
    return res;
}
int main() {
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cerr << "Failed to initialize Winsock" << endl;
        return 1;
    }

    SOCKET serverSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (serverSocket == INVALID_SOCKET) {
        cerr << "Failed to create server socket" << endl;
        WSACleanup();
        return 1;
    }

    sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(12345); // 서버 포트 번호
    serverAddr.sin_addr.s_addr = INADDR_ANY;

    if (bind(serverSocket, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        cerr << "Failed to bind server socket" << endl;
        closesocket(serverSocket);
        WSACleanup();
        return 1;
    }

    if (listen(serverSocket, SOMAXCONN) == SOCKET_ERROR) {
        cerr << "Failed to listen on server socket" << endl;
        closesocket(serverSocket);
        WSACleanup();
        return 1;
    }

    cout << "Server is listening on port 12345" << endl;

    sockaddr_in clientAddr;
    int clientAddrSize = sizeof(clientAddr);


    SOCKET clientSocket1 = accept(serverSocket, (struct sockaddr*)&clientAddr, &clientAddrSize);
    if (clientSocket1 == INVALID_SOCKET) {
        cerr << "Failed to accept client connection" << endl;
        closesocket(serverSocket);
        WSACleanup();
        return 1;
    }

    cout << "Client 1 connected" << endl;

    client[0].clientSock = clientSocket1;
    client[0].playerNum = 1;
    client[0].isStart = false;

    client[0].black_card = { 0,2,4,6,8 };
    client[0].white_card = { 1,3,5,7 };
    string strNum1 = to_string(client[0].playerNum);
    send(client[0].clientSock, strNum1.c_str(), strNum1.size(), 0);


    // Accept the second client and assign playerNumber 2
    SOCKET clientSocket2 = accept(serverSocket, (struct sockaddr*)&clientAddr, &clientAddrSize);
    if (clientSocket2 == INVALID_SOCKET) {
        cerr << "Failed to accept client connection" << endl;
        closesocket(serverSocket);
        WSACleanup();
        return 1;
    }

    cout << "Client 2 connected" << endl;

    client[1].clientSock = clientSocket2;
    client[1].playerNum = 2;
    client[1].isStart = false;

    client[1].black_card = { 0,2,4,6,8 };
    client[1].white_card = { 1,3,5,7 };
    client[1].playerNum = htonl(2);
    string strNum2 = to_string(client[1].playerNum);
    send(client[1].clientSock, strNum2.c_str(), strNum2.size(), 0);


    Setting set;

    // Start the game with both clients
    thread thread[2];
    for (int i = 0; i < CLIENT_CNT; i++) {
        thread[i] = std::thread(startGame, std::ref(client[i]), std::ref(set));
    }
    // Wait for threads to finish

    int winer = set.turn_start;
   
    for (int i = 0; i < CLIENT_CNT; i++) {
        thread[i].join();
    }

    closesocket(serverSocket);
    closesocket(clientSocket1);
    closesocket(clientSocket2);
    WSACleanup();

    return 0;
}

void handle(Client &now_client,Setting& set) {
    char buffer[1024];
    int received = recv(now_client.clientSock, buffer, sizeof(buffer), 0);
    if (received == SOCKET_ERROR || received == 0) {
        cerr << "Client disconnected" << endl;
        closesocket(now_client.clientSock);
        exit(1);
    }

    buffer[received] = '\0'; // 수신된 데이터를 문자열로 만들기

    lock_guard<mutex> lock(now_client.mutex_client);

    if (strcmp(buffer, "GameStart") == 0) {
        cout << "Game is starting!" << endl;

        now_client.isStart = true;

        lock_guard<mutex> lock1(now_client.mutex_client);

        if (client[0].isStart && client[1].isStart) {
            sendMsg("SOUND", 0);
            sendMsg("GAMESTART", CLIENT_CNT);
            sendMsg("* ************************************************\n", CLIENT_CNT);
            sendMsg(" *                 < 카드 분배>                  *\n", CLIENT_CNT);
            sendMsg(" *************************************************\n", CLIENT_CNT);
            
            
            sendMsg("> 딜러가 카드를 분배합니다", CLIENT_CNT);
            Sleep(1000);
            sendMsg(" 3 ", CLIENT_CNT);
            Sleep(1000);
            sendMsg(" 2 ", CLIENT_CNT);
            Sleep(1000);
            sendMsg(" 1 ", CLIENT_CNT);

            set.turn_start = play_turn();
        }
    }
    // 다른 서버 메시지에 대한 처리 추가
}

void startGame(Client &client, Setting& set) {
    send(client.clientSock, "GameStart", strlen("GameStart"), 0);

    // 클라이언트로부터 게임 시작 응답 받기
    char buffer[1024];
    int received = recv(client.clientSock, buffer, sizeof(buffer), 0);

    if (received == SOCKET_ERROR || received == 0) {
        cerr << "Client 연결이 끊어짐" << endl;
        closesocket(client.clientSock);
        exit(1);
    }

    buffer[received] = '\0'; // 수신된 데이터를 문자열로 만들기

    lock_guard<mutex> lock_client(client.mutex_client);


    // 클라이언트의 응답에 따라 게임 시작 또는 종료6
    if (strcmp(buffer, "GAMESTART") == 0) {
        cout << "Game 시작!" << endl;
        string turn = "TURNSTART " + to_string(set.turn_start);
        send(client.clientSock, turn.c_str(), turn.size(), 0);
    }
    else {
        cout << "Client chose to quit" << endl;
    }
}


void sendMsg(const char* msg, int idx) {
    for (int i = 0; i < CLIENT_CNT; i++) {
        send(client[i].clientSock, msg, MAX_SIZE, 0);
    }
}
    

void game_handle(Client& new_client, Setting& set, int& round, int& p1_score, int& p2_score) {
    char buffer[1024];
    int p1_num = 0, p2_num = 0;

    // 플레이어로부터 숫자를 받음
    int received = recv(new_client.clientSock, buffer, sizeof(buffer), 0);
    if (received <= 0) {
        cerr << "Error in receiving data or client disconnected" << endl;
        return;
    }

    buffer[received] = '\0';

    // 플레이어의 숫자 선택 처리
    if (strncmp(buffer, "NUM ", 4) == 0) {
        int choice = atoi(buffer + 4);
        if (new_client.playerNum == 1) {
            p1_num = choice;
        }
        else {
            p2_num = choice;
        }
    }

    string result = "";

    // 라운드 결과 계산
    if (p1_num != 0 && p2_num != 0) {  // 두 플레이어 모두 선택했을 때
        if (p1_num > p2_num) {
            p1_score++;
            result = "Winner is Player 1";
        }
        else if (p1_num < p2_num) {
            p2_score++;
            result = "Winner is Player 2";
        }
        else {
            p1_score++;
            p2_score++;
            result = "Draw";
        }

        // 다음 라운드 시작 또는 게임 종료 처리
        round++;
        if (round >= 9) {
            // 게임 종료 로직
            if (p1_score != p2_score) {
                if (p1_num > p2_num) {
                    p1_score++;
                    result = "Winner is Player 1";
                }
                else if (p1_num < p2_num) {
                    p2_score++;
                    result = "Winner is Player 2";
                }
            }
            else {
               
                round = 0;
                p1_score = 0;
                p2_score = 0;
                result = "Draw";
            }
        }

        // 라운드 초기화
        p1_num = 0;
        p2_num = 0;

        send(client->clientSock, result.c_str(), sizeof(result.c_str()), 0);
    }
}
