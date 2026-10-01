# Ironwall ONVIF Solution

.NET 8에서 ONVIF 카메라의 장치 정보, 미디어 프로파일, RTSP 주소와 PTZ 기능을 사용하는 라이브러리입니다. SOAP 클라이언트 생성과 모델 구성을 분리하고, 호출 목적에 따라 초기화 범위를 선택하도록 구성했습니다.

## 주요 기능

- `InitializeDeviceAsync`: 장치 기본 정보 초기화
- `InitializeProfileAsync`: 장치와 미디어 프로파일, 스트림 주소 초기화
- `InitializeFullAsync`: PTZ·이미징·프리셋 정보를 포함한 초기화
- PTZ 이동·정지, 홈 위치와 프리셋 조회·설정·이동

## 구성

- [Services](Services): ONVIF 서비스 인터페이스와 호출 처리
- [Factories](Factories): 서비스 클라이언트 생성
- [OnvifModelBuilder](Services/OnvifModelBuilder.cs): 카메라 모델 구성
- [Models](Models): 장치와 프로파일 데이터
- [Tests](Tests): 서비스와 모델 구성 테스트 코드

## 개발 환경

Windows / .NET 8, WPF 참조, Autofac, System.ServiceModel, Newtonsoft.Json을 사용합니다. 테스트 관련 의존성은 xUnit과 Moq입니다.

현재 프로젝트는 저장소 밖의 `Ironwall.Dotnet.Libraries.Base`와 `Ironwall.Dotnet.Libraries.OnvifSolution.Base`를 참조합니다. 해당 소스를 준비하고 프로젝트 참조 경로를 맞춘 뒤 빌드해야 합니다. 카메라의 ONVIF 지원 서비스와 인증 설정에 따라 사용 가능한 기능이 달라집니다.

테스트 소스가 포함되어 있으나, README 정리 과정에서 실제 카메라 연결이나 테스트 실행을 새로 검증하지는 않았습니다. 기존 Sensorway / Ironwall 코드 및 ONVIF 생성 프록시의 출처 표기를 유지합니다.
