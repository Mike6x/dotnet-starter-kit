variable "name" {
  type        = string
  description = "Bucket name."
}

variable "tags" {
  type        = map(string)
  description = "Tags to apply to the bucket."
  default     = {}
}

variable "enable_public_read" {
  type        = bool
  description = "Set to true to allow public read on the specified prefix via bucket policy."
  default     = false
}

variable "public_read_prefix" {
  type        = string
  description = "Prefix to allow public read (e.g., uploads/). Leave empty to disable public policy."
  default     = "uploads/"
}

variable "enable_cloudfront" {
  type        = bool
  description = "Set to true to provision a CloudFront distribution in front of the bucket."
  default     = false
}

variable "cloudfront_price_class" {
  type        = string
  description = "CloudFront price class."
  default     = "PriceClass_100"
}

variable "cloudfront_comment" {
  type        = string
  description = "Optional comment for the CloudFront distribution."
  default     = ""
}
