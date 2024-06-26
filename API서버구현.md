# API서버 구현

---

# MVC 패턴

## Model

Django에서의 모델과 동일하다. 데이터와 비즈니스 로직을 관리한다. 애플리케이션의 데이터 관련 로직을 처리하며, 데이터베이스와의 상호작용을 담당하는 구성 요소이다. 요청/응답으로 사용할 클래스를 정의한다.

## View

시각적으로 표현하는 인터페이스. 컨트롤러로부터 데이터를 받아 사용자에게 보여주는 형태로 가공. HTML, CSS, Javascript로 구현한다.

## Controller

Django에서의 View와 같은 역할. 입력을 받고 처리한다. 사용자의 입력을 받아 모델을 업데이트하고, 적절한 뷰를 선택하여 응답을 돌려준다.

## Repository

데이터베이스 IO를 처리하는 모든 컨트롤 로직을 이곳에 모아놓고 관리한다. 

## DAO (Data Transfer Object)

DTO는 "Data Transfer Object"의 약자로, 데이터 전송 객체라고도 한다. 이는 여러 계층 간 데이터를 전송하는 데 사용되는 객체로, 주로 데이터베이스에서 데이터를 조회하여 서비스나 컨트롤러 계층으로 데이터를 전달할 때 사용한다. 여러 데이터 필드를 하나의 객체로 캡슐화하여 네트워크를 통한 데이터 전송을 최적화하는 데 도움을 준다. DTO는 로직을 포함하지 않고 순수하게 데이터를 전달하는 데 사용되며, 일반적으로 getter와 setter 메소드만 포함한다.

## DTO (Data Access Object)

DAO는 "Data Access Object"의 약자로, 데이터베이스의 데이터에 접근하는 객체를 말한다. DAO는 데이터베이스의 CRUD(Create, Read, Update, Delete) 작업을 캡슐화하여, 이러한 작업을 수행하는 특정 메소드를 제공한다. DAO를 사용함으로써 애플리케이션의 비즈니스 로직과 데이터 액세스 로직을 분리할 수 있으며, 이는 코드의 재사용성과 유지보수성을 향상시킨다.

---

# DI 패턴 (의존성 주입, Dependency Injection)

`Program.cs` 파일에 서비스를 등록하고, 이를 클래스의 생성자로 받아들여 적용한다. A 클래스에 B 클래스를 의존성 주입하게 되면 결과적으로 A클래스는 B클래스의 기능에 의존한다. 또한 A클래스는 B클래스의 내부 구현에 대해 알 필요가 없게 된다. 이는 코드의 결합도를 낮추고 유연한 개발을 가능하게 한다.

### 1. 결합도 감소 (Reduced Coupling)

인터페이스를 사용하면, 구체적인 클래스가 아닌 인터페이스에 의존하게 된다. 이것은 구현 세부사항에서 추상화로 의존성을 이동시키는 것을 의미한다. 결과적으로, 하나의 모듈이나 클래스가 다른 클래스의 내부 구현에 덜 의존하게 되어, 시스템 전체의 결합도가 감소한다. 결합도가 낮아지면, 코드를 변경하거나 업데이트할 때 그 영향을 받는 범위가 줄어들어 유지 관리가 더 쉬워진다.

### 2. 테스트 용이성 (Ease of Testing)

인터페이스를 통해 의존성을 주입하면, 단위 테스트 중에 실제 구현 대신 모의 객체(Mock Object)나 가짜 구현(Fake Implementation)을 사용할 수 있다. 예를 들어, 데이터베이스 액세스 레이어가 인터페이스를 통해 주입되는 경우, 테스트 중에는 데이터베이스 호출을 수행하지 않고 데이터를 반환하도록 설계된 가짜 레이어를 주입할 수 있다. 이로 인해 테스트가 더 빠르고, 덜 복잡해지며, 외부 시스템에 의존하지 않게 된다.

### 3. 유연성과 확장성 (Flexibility and Scalability)

인터페이스를 사용하면 실행 시에 다양한 구현을 쉽게 교체할 수 있다. 예를 들어, 응용 프로그램이 다양한 유형의 데이터 저장소(예: SQL 데이터베이스, NoSQL 데이터베이스, 파일 시스템 등)를 지원해야 할 경우, 각 저장소 유형에 대한 구체적인 구현을 인터페이스를 통해 주입하면, 애플리케이션의 나머지 부분은 변경 없이 유지할 수 있다.

### 4. 원칙의 준수 (Principle Compliance)

인터페이스를 사용하는 의존성 주입은 "개방/폐쇄 원칙"과 "의존성 역전 원칙"과 같은 SOLID 디자인 원칙을 준수한다. 개방/폐쇄 원칙은 소프트웨어 엔티티(클래스, 모듈, 함수 등)가 확장에는 열려 있어야 하지만, 변경에는 닫혀 있어야 한다고 말한다. 인터페이스를 사용하면 기존 코드를 변경하지 않고도 새로운 기능을 쉽게 추가할 수 있다. 의존성 역전 원칙은 고수준 모듈이 저수준 모듈에 의존해서는 안 되며, 둘 다 추상화에 의존해야 한다고 말한다. 인터페이스를 사용함으로써 이 원칙을 따를 수 있다.

결론적으로, 의존성 주입에서 인터페이스를 사용하는 것은 소프트웨어 개발에서 더 나은 설계 결정을 내리는 데 도움이 된다. 이로 인해 생성된 코드는 더 견고하고, 유연하며, 테스트 및 유지 관리가 용이해진다.

### 서비스 수명 주기

.NET의 DI 시스템에서 각 서비스 또는 객체가 어떻게 생성되고 관리되는지를 정의한다. `Program.cs` 파일에 서비스를 등록할 때 적용한다.

Singleton, Transient, 그리고 Scoped와 같은 용어들은 서비스의 수명 주기 또는 수명 관리 전략을 지칭한다. 이들은 "서비스 수명 주기(Service Lifetimes)" 또는 "의존성 주입 수명 관리(Dependency Injection Lifetimes)" 옵션이라고 불린다. 이 용어들은 .NET의 의존성 주입(Dependency Injection, DI) 시스템에서 각 서비스 또는 객체가 어떻게 생성되고 관리되는지를 정의한다.

### 각 수명 주기의 설명

1. **Singleton**
    - Singleton 수명 주기는 애플리케이션이 시작될 때 생성되며, 애플리케이션의 전체 수명 동안 하나의 인스턴스만 존재한다. 즉, 서비스가 요청될 때마다 동일한 인스턴스가 반복해서 사용됩니다. 이는 공유 자원이나, 전역 상태 관리 등에 사용될 수 있다.
2. **Transient**
    - Transient 수명 주기는 서비스가 요청될 때마다 새로운 인스턴스가 생성된다. 각 요청은 고유한 인스턴스를 받으며, 이 인스턴스는 요청 처리가 끝나면 해제된다. 이는 주로 각 요청에서 독립적으로 동작해야 하는 서비스에 사용된다.
3. **Scoped**
    - Scoped 수명 주기는 하나의 요청이 시작될 때 생성되고, 요청이 끝날 때까지 유지된다. 요청 내에서는 하나의 인스턴스가 공유되지만, 다른 요청 간에는 서로 다른 인스턴스가 사용된다. 이는 주로 웹 애플리케이션에서 요청 기반의 데이터를 공유할 때 사용된다.

### 사용 예

.NET Core나 .NET 5/6 등에서 의존성 주입을 설정할 때, `Startup.cs` 파일의 `ConfigureServices` 메서드에서 이러한 수명 주기를 사용하여 서비스를 등록할 수 있다. 예를 들어:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<ISingletonService, SingletonService>();
    services.AddTransient<ITransientService, TransientService>();
    services.AddScoped<IScopedService, ScopedService>();
}

```

이러한 설정을 통해, 개발자는 애플리케이션의 필요에 따라 객체 생성과 메모리 관리를 효율적으로 제어할 수 있으며, 서비스의 동작 방식을 더욱 유연하게 구성할 수 있다.

---

# Database

Hive Server용 MySQL과 Redis, Game Server용 MySQL과 Redis로 총 4개의 데이터베이스 사용했다.

## Redis

- CloudStructures 라이브러리 사용
- 키-값 쌍으로 데이터를 저장
- 기본적으로 Thread safe하기 때문에 싱글톤으로 구현

## MySQL

- SqlKata 라이브러리 사용
- Transient로 구현