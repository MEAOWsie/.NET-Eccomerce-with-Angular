pipeline {
  agent any

  environment {
    PATH  = "/usr/local/bin:/usr/bin:/bin:/snap/bin:${env.PATH}"
    IMAGE = "skinet-client"
    TAG   = "${BUILD_NUMBER}"
  }

  stages {
    stage('Checkout') {
      steps {
        checkout scm
      }
    }

    stage('Verify tools') {
      steps {
        sh 'docker --version'
        sh 'kubectl version --client'
        sh 'minikube version'
        sh 'kubectl get nodes'
      }
    }

    stage('Build image') {
      steps {
        sh 'docker build -t $IMAGE:$TAG ./client'
      }
    }

    stage('Load into cluster') {
      steps {
        sh 'minikube -p skinet image load $IMAGE:$TAG'
      }
    }

    stage('Deploy') {
      steps {
        sh 'kubectl set image deployment/frontend frontend=$IMAGE:$TAG'
        sh 'kubectl rollout status deployment/frontend --timeout=180s'
      }
    }

    stage('Smoke test') {
      steps {
        sh 'curl -sf http://192.168.58.2:30304/ > /dev/null'
        sh 'curl -sf http://192.168.58.2:30304/api/products > /dev/null'
      }
    }
  }

  post {
    success { echo "Déploiement réussi : ${IMAGE}:${TAG}" }
    failure { echo "Échec du pipeline" }
  }
}
