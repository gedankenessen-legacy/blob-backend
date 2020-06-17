#!/usr/bin/python3
import os
import sys

disableQuestions = False

def main():
    global disableQuestions

    if len(sys.argv) > 1:
        disableQuestions = sys.argv[1]

    containerName = "blob_backend"
    
    # Ask for custom tag.
    tag = askInput("Specifiy Tag [latest]: ", "latest")

    # Print create cmd to validate
    dockerCMD = f"docker run --restart always -d -p 5001:80 --ip 10.10.10.2 --name {containerName} --network blob blobcd/blob:{tag}"
    print(dockerCMD)

    # Validate cmd and create new container, pull latest image:tag 
    if askYesOrNo("Befehl korrekt? [j, N]: "):
        # Remove old container if any.
        oldContainerExists = os.system(f"docker ps | grep {containerName}")
        if oldContainerExists is 0:
            removeOld = askYesOrNo("Remove old container? [j, N]: ")
            if removeOld is True:
                os.system(f"docker rm -f {containerName}")
                
        os.system(f"docker pull blobcd/blob:{tag}")
        os.system(dockerCMD)

def askYesOrNo(text):
    global disableQuestions

    if disableQuestions is False:
        yes = {'yes','y', 'ye', 'j'}
        no = {'no','n', ''}

        choice = input(text).lower()
        if choice in yes:
            return True
        elif choice in no:
            return False
        else:
            print("Please respond with 'yes' or 'no'")
    else:
        return True

def askInput(question, default="latest"):
    global disableQuestions
    if disableQuestions is False:
        answer = input(question)
        if answer is "":
            return default
        else:
            return answer
    else:
        return "b_latest"

if __name__ == "__main__":
    main()
