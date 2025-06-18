# 🗣️ VR 영어 교육 시뮬레이션
> ### PBL I 프로젝트 (3인)
> ### 주제 : VR을 이용한 영어 교육 시뮬레이션 프로그램
> ### 역할 : 클라이언트 - 서버 구조 설계, STT 및 TTS API 연동, 데이터 흐름 처리
<img src="https://github.com/user-attachments/assets/b753d2d6-f845-42ce-b91b-148d2b711d85" width="700"/>
<br><br> 

## 💻 개발 환경
- 클라이언트 개발 플랫폼 : Unity <br> 
- 개발 언어 : C#, Python <br> 
- IDE : Unity, Visual Studio Code (Flask Server)
- DB : MySQL
- 통신 방식: WebSocket
<br><br>

##  주요 기능
### 1️⃣ 사용자와의 대화
TTS(Text-to-Speech)와 STT(Speech-to-Text) 기술을 통해 사용자가 음성으로 질문에 응답하거나 피드백을 받을 수 있는 대화 환경을 제공한다. <br>
이러한 상호작용 경험을 통해 사용자는 영어 말하기 및 듣기 능력을 효과적으로 향상시킬 수 있다.
<br>

### 2️⃣ 영어 문법 교정 및 표현 추천 AI
rompt Engineering을 통해 학습된 AI 언어 모델은 사용자의 영어 문장에 대한 문법 오류를 실시간으로 교정하며,  <br>
상황에 알맞은 표현 추천을 통해 보다 정확한 문장을 구사할 수 있도록 돕는다.
<br>

### 3️⃣ 시뮬레이션 
사용자가 상점과 면접 상황에서 NPC와 대화하며 영어 회화를 연습할 수 있는 환경을 제공하여 실제 상황에서의 영어 유창성 및 자신감을 향상시킬 수 있다.  <br>
상점 시뮬레이션에서는 물건을 집어 장바구니에 담는 등의 상호작용을 더해 더욱 몰입감 있는 환경을 구현하였다.
<br>

<br><br> 

## 아키텍처
<img src="https://github.com/user-attachments/assets/eb2f8c66-bd1d-471d-8bbd-a3b2ffd8097d" width="700"/>
<br><br> 


## 실행 화면
<img src="https://github.com/user-attachments/assets/b753d2d6-f845-42ce-b91b-148d2b711d85" width="600"/>
<br>

<img src="https://github.com/user-attachments/assets/1100a057-27e3-4d3c-a4c9-9d09a634474a" width="600"/>

<br><br>

## 트러블 슈팅
문제: TTS 결과를 처리하는 과정에서 평균 9초의 응답 지연 발생

원인: 텍스트 데이터를 DB에 저장한 뒤 클라이언트가 다시 조회하는 구조에서 병목 현상 발생

해결: WebSocket 기반의 실시간 통신 구조로 변경하여 DB I/O 제거

결과: 평균 응답 시간을 약 5초로 단축 (약 40% 개선), 사용자 경험 향상

## 수상 
- 2024학년도 연구성과 경진대회 및 제 33회 소프트웨어 전시회 최우수상


