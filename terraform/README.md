Terraform infrastructure for deploying the fullstackhero .NET starter kit to AWS using ECS Fargate.

Structure:
- bootstrap: creates the remote state S3 bucket.
- modules: reusable building blocks (network, ECS, RDS, ElastiCache, S3).
- envs: environment and region specific stacks that compose modules.

Environments and regions:
- Each environment (dev, staging, prod) can have one or more regions.
- The pattern is envs/<env>/<region>.

Workflow:
1. From terraform/bootstrap:
   - terraform init
   - terraform apply -var="region=<state_region>" -var="bucket_name=<state_bucket_name>"
2. For each envs/<env>/<region>:
   - Update backend.tf with the created state bucket name and a unique key.
   - terraform init
   - terraform plan -var-file="<env>.<region>.tfvars"
   - terraform apply -var-file="<env>.<region>.tfvars"

Multi-region:
- To add another region, copy an existing region folder (for example envs/dev/us-east-1 to envs/dev/eu-central-1) and adjust:
  - backend.tf key and region
  - *.tfvars region, CIDRs, and names as needed.
