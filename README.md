# MozaCounter

Diablo 2 Resurrected 카운터 프로그램

## 기능

- ✅ 설정 가능한 트리거 키로 카운트다운 시작/리셋
- ✅ 커스터마이징 가능한 폰트 설정 (종류, 크기, 색상)
- ✅ 폰트 배경 효과 설정 (두께, 색상)
- ✅ 마우스 위치 추적을 통한 카운터 위치 설정
- ✅ 설정값 자동 저장 및 불러오기
- ✅ 윈도우 위치 기억
- ✅ 실행/중지 상태 관리

## 요구사항

- .NET 9.0 Runtime

## 사용 방법

1. 프로그램 실행
2. "설정 열기" 버튼을 클릭하여 설정
   - 폰트 종류, 크기, 색상 설정
   - 폰트 배경 두께, 색상 설정
   - 카운터 위치 설정 (마우스 추적 기능 사용 가능)
   - 카운트 시작 시간 설정
   - 트리거 키 설정
3. "실행" 상태에서 트리거 키를 누르면 카운트다운 시작
4. 카운트다운 중 트리거 키를 다시 누르면 리셋

## 빌드

```bash
dotnet build
```

## 단일 실행 파일 생성

```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

## 라이선스

Copyright© Lodofin32 All rights reserved.
