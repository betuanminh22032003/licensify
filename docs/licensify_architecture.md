# 🧠 Licensify – Technology & System Architecture

## 🔧 Technology Stack

| Layer          | Tech Stack                                |
| -------------- | ----------------------------------------- |
| Frontend       | React + NextUI                            |
| Backend        | ASP.NET Core (.NET 8)                     |
| Database       | PostgreSQL                                |
| Cache          | Redis                                     |
| Message Broker | RabbitMQ                                  |
| Logging        | Serilog, MongoDB/Elasticsearch (optional) |
| DevOps         | Docker, Kubernetes, Helm, GitHub Actions  |
| Monitoring     | Prometheus, Grafana                       |

---

## 🧱 System Architecture

```mermaid
flowchart TB
    subgraph User
        A[Admin User]
    end

    subgraph Frontend
        B[Admin Panel]
    end

    subgraph Services
        Auth[Auth Service] --> Redis[Redis Cache]
        Product[Product Service]
        License[License Service] -->|Emit| MQ[RabbitMQ]
        Audit[Audit Service] -->|Subscribe| MQ
    end

    subgraph Infra
        DB1[(PostgreSQL)]
        DB2[(MongoDB / Elasticsearch)]
        MQ
        Monitor[Prometheus + Grafana]
    end

    A --> B
    B --> Auth
    B --> Product
    B --> License

    Auth --> DB1
    Product --> DB1
    License --> DB1
    Audit --> DB2

    Auth --> Monitor
    Product --> Monitor
    License --> Monitor
    Audit --> Monitor
```

---

## 📦 Deployment Architecture

* All services are containerized using Docker.
* Each service has its own Helm chart for K8s deployment.
* Configuration managed via Secrets and ConfigMaps.
* GitHub Actions handles CI (build/test) and CD (deploy).
* PostgreSQL and Redis deployed as stateful workloads.
* Optional: External MongoDB or Elasticsearch if audit logging is enabled.

---

👊 This architecture supports modular scaling, observability, CI/CD, and is cloud-native by design. Ready for SaaS or self-hosted licensing.
