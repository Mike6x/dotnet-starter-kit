variable "environment" {
  type        = string
  description = "Environment name."
}

variable "region" {
  type        = string
  description = "AWS region."
}

variable "vpc_cidr_block" {
  type        = string
  description = "CIDR block for the VPC."
}

variable "public_subnets" {
  description = "Public subnet definitions."
  type = map(object({
    cidr_block = string
    az         = string
  }))
}

variable "private_subnets" {
  description = "Private subnet definitions."
  type = map(object({
    cidr_block = string
    az         = string
  }))
}

variable "app_s3_bucket_name" {
  type        = string
  description = "S3 bucket for application data."
}

variable "app_s3_enable_public_read" {
  type        = bool
  description = "Whether to enable public read on uploads prefix."
  default     = false
}

variable "app_s3_public_read_prefix" {
  type        = string
  description = "Prefix to allow public read (e.g., uploads/)."
  default     = "uploads/"
}

variable "app_s3_enable_cloudfront" {
  type        = bool
  description = "Whether to provision a CloudFront distribution for the app bucket."
  default     = true
}

variable "app_s3_cloudfront_price_class" {
  type        = string
  description = "Price class for CloudFront."
  default     = "PriceClass_100"
}

variable "db_name" {
  type        = string
  description = "Database name."
}

variable "db_username" {
  type        = string
  description = "Database admin username."
}

variable "db_password" {
  type        = string
  description = "Database admin password."
}

variable "api_container_image" {
  type        = string
  description = "API container image."
}

variable "api_container_port" {
  type        = number
  description = "API container port."
}

variable "api_cpu" {
  type        = string
  description = "API CPU units."
}

variable "api_memory" {
  type        = string
  description = "API memory."
}

variable "api_desired_count" {
  type        = number
  description = "Desired API task count."
}

variable "blazor_container_image" {
  type        = string
  description = "Blazor container image."
}

variable "blazor_container_port" {
  type        = number
  description = "Blazor container port."
}

variable "blazor_cpu" {
  type        = string
  description = "Blazor CPU units."
}

variable "blazor_memory" {
  type        = string
  description = "Blazor memory."
}

variable "blazor_desired_count" {
  type        = number
  description = "Desired Blazor task count."
}
