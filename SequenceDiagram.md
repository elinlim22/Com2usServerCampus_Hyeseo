title Hive Server & Game Server

```mermaid
sequenceDiagram
    actor 유저
    participant 게임서버
    participant 하이브서버
    participant GameRedis
    participant GameMySQL
    participant HiveRedis
    participant HiveMySQL


    유저->>하이브서버: 계정생성 요청(이메일, 비밀번호)
    하이브서버->>HiveMySQL: 유저 정보 저장(ID, 이메일, 비밀번호)
    하이브서버->>HiveRedis: 유저 토큰 저장(ID, 토큰)
    하이브서버-->>유저: 계정생성 응답(ID, 토큰)
    유저->>게임서버: 로그인 요청(ID, 토큰)
    게임서버->>하이브서버: 유효성 검사(ID, 토큰)
    하이브서버->>HiveRedis: 토큰 검색(ID, 토큰)
    HiveRedis-->>하이브서버: 토큰 검색 결과
    하이브서버-->>게임서버: 응답(유효성 여부)
    alt 토큰 불일치
    게임서버-->>유저: 로그인 실패
    end
    게임서버->>GameRedis: 유저 토큰 저장(ID, 토큰)
    게임서버->>GameMySQL: 게임데이터 불러오기
    GameMySQL-->>게임서버: 게임데이터
    alt 게임데이터 없음
    게임서버->>GameMySQL: 게임데이터 생성
    end

