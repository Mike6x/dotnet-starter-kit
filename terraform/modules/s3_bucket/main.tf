terraform {
  required_version = ">= 1.5.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

resource "aws_s3_bucket" "this" {
  bucket = var.name

  tags = var.tags
}

# Enforce bucket owner ownership (disables ACLs).
resource "aws_s3_bucket_ownership_controls" "this" {
  bucket = aws_s3_bucket.this.id

  rule {
    object_ownership = "BucketOwnerEnforced"
  }
}

resource "aws_s3_bucket_versioning" "this" {
  bucket = aws_s3_bucket.this.id

  versioning_configuration {
    status = "Enabled"
  }
}

resource "aws_s3_bucket_server_side_encryption_configuration" "this" {
  bucket = aws_s3_bucket.this.id

  rule {
    apply_server_side_encryption_by_default {
      sse_algorithm = "AES256"
    }
  }
}

resource "aws_s3_bucket_public_access_block" "this" {
  bucket                  = aws_s3_bucket.this.id
  block_public_acls       = true
  ignore_public_acls      = true
  block_public_policy     = var.enable_public_read ? false : true
  restrict_public_buckets = var.enable_public_read ? false : true
}

resource "aws_cloudfront_origin_access_control" "this" {
  count                            = var.enable_cloudfront ? 1 : 0
  name                             = "${aws_s3_bucket.this.bucket}-oac"
  description                      = "Access control for ${aws_s3_bucket.this.bucket}"
  origin_access_control_origin_type = "s3"
  signing_behavior                 = "always"
  signing_protocol                 = "sigv4"
}

resource "aws_cloudfront_distribution" "this" {
  count = var.enable_cloudfront ? 1 : 0

  enabled             = true
  comment             = var.cloudfront_comment != "" ? var.cloudfront_comment : "Public assets for ${aws_s3_bucket.this.bucket}"
  price_class         = var.cloudfront_price_class
  default_root_object = ""

  origin {
    domain_name              = aws_s3_bucket.this.bucket_regional_domain_name
    origin_id                = "s3-${aws_s3_bucket.this.bucket}"
    origin_access_control_id = aws_cloudfront_origin_access_control.this[0].id
  }

  default_cache_behavior {
    target_origin_id       = "s3-${aws_s3_bucket.this.bucket}"
    viewer_protocol_policy = "redirect-to-https"
    allowed_methods        = ["GET", "HEAD"]
    cached_methods         = ["GET", "HEAD"]

    forwarded_values {
      query_string = false
      cookies {
        forward = "none"
      }
    }

    compress = true
  }

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  viewer_certificate {
    cloudfront_default_certificate = true
  }

  tags = var.tags
}

locals {
  bucket_policy_statements = concat(
    var.enable_public_read && length(var.public_read_prefix) > 0 ? [
      {
        Sid       = "AllowPublicReadUploads"
        Effect    = "Allow"
        Principal = "*"
        Action    = ["s3:GetObject"]
        Resource  = "arn:aws:s3:::${aws_s3_bucket.this.bucket}/${var.public_read_prefix}*"
      }
    ] : [],
    var.enable_cloudfront ? [
      {
        Sid      = "AllowCloudFrontRead"
        Effect   = "Allow"
        Principal = {
          Service = "cloudfront.amazonaws.com"
        }
        Action   = ["s3:GetObject"]
        Resource = "arn:aws:s3:::${aws_s3_bucket.this.bucket}/*"
        Condition = {
          StringEquals = {
            "AWS:SourceArn" = aws_cloudfront_distribution.this[0].arn
          }
        }
      }
    ] : []
  )
}

resource "aws_s3_bucket_policy" "this" {
  count  = length(local.bucket_policy_statements) > 0 ? 1 : 0
  bucket = aws_s3_bucket.this.id
  policy = jsonencode({
    Version   = "2012-10-17"
    Statement = local.bucket_policy_statements
  })
}

output "bucket_name" {
  value = aws_s3_bucket.this.id
}

output "cloudfront_domain_name" {
  value       = var.enable_cloudfront ? aws_cloudfront_distribution.this[0].domain_name : ""
  description = "CloudFront domain for public access (when enabled)."
}

output "cloudfront_distribution_id" {
  value       = var.enable_cloudfront ? aws_cloudfront_distribution.this[0].id : ""
  description = "CloudFront distribution ID (when enabled)."
}
