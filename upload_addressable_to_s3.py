import os
import boto3
from botocore.exceptions import ClientError

# ✅ 사용자 설정
AWS_REGION = "ap-northeast-2"
BUCKET_NAME = "whawoo-game-addressable"
S3_FOLDER = "Android"  # Addressable 플랫폼 폴더명
# Jenkins 환경이면 ENV 사용
LOCAL_BUILD_PATH = os.environ.get("WORKSPACE", os.getcwd())
LOCAL_BUILD_PATH = os.path.join(LOCAL_BUILD_PATH, "BuildPath", "Android")

# 🔥 CloudFront 캐시 무효화 (선택사항)
ENABLE_CLOUDFRONT_INVALIDATION = True
CLOUDFRONT_DISTRIBUTION_ID = "E3MTBTPWJ9RK4Q"

# --------------------------------------------------------------------------------------------------

def delete_s3_folder(s3, bucket_name, folder):
    print(f"Deleting existing files in s3://{bucket_name}/{folder}/...")
    bucket = s3.Bucket(bucket_name)
    objects_to_delete = bucket.objects.filter(Prefix=folder + "/")
    deleted = objects_to_delete.delete()
    print(f"Deleted: {deleted}")

def upload_directory(s3_client, local_path, bucket, s3_path):
    print(f"Uploading {local_path} to s3://{bucket}/{s3_path}/...")
    for root, dirs, files in os.walk(local_path):
        for file in files:
            local_file = os.path.join(root, file)
            relative_path = os.path.relpath(local_file, local_path).replace("\\", "/")
            s3_file = f"{s3_path}/{relative_path}"
            print(f"Uploading {s3_file}")
            s3_client.upload_file(local_file, bucket, s3_file)
    print("Upload completed.")

def invalidate_cloudfront(distribution_id, paths=["/*"]):
    client = boto3.client('cloudfront')
    print("Creating CloudFront invalidation...")
    response = client.create_invalidation(
        DistributionId=distribution_id,
        InvalidationBatch={
            'Paths': {
                'Quantity': len(paths),
                'Items': paths
            },
            'CallerReference': str(hash(str(paths)))
        }
    )
    print(f"Invalidation created: {response['Invalidation']['Id']}")

# --------------------------------------------------------------------------------------------------

if __name__ == "__main__":
    session = boto3.Session(region_name=AWS_REGION)
    s3 = session.resource('s3')
    s3_client = session.client('s3')

    try:
        delete_s3_folder(s3, BUCKET_NAME, S3_FOLDER)
        upload_directory(s3_client, LOCAL_BUILD_PATH, BUCKET_NAME, S3_FOLDER)

        if ENABLE_CLOUDFRONT_INVALIDATION:
            invalidate_cloudfront(CLOUDFRONT_DISTRIBUTION_ID)

    except ClientError as e:
        print(f"An error occurred: {e}")
