import json
import os
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity

realpath = os.path.realpath(os.getcwd() + "\\wwwroot\\Processes")
listdir = os.listdir(realpath)
student_files = [doc for doc in listdir if doc.endswith('.txt')]
student_notes = [open((realpath + "\\" + _file), encoding='utf-8').read() for _file in student_files]



def vectorize(Text): return TfidfVectorizer().fit_transform(Text).toarray()
def similarity(doc1, doc2): return cosine_similarity([doc1, doc2])


vectors = vectorize(student_notes)
s_vectors = list(zip(student_files, vectors))
plagiarism_results = list()
plagiarism_results_set = set()



def check_plagiarism():
    global s_vectors
    for student_a, text_vector_a in s_vectors:
        new_vectors = s_vectors.copy()
        current_index = new_vectors.index((student_a, text_vector_a))
        del new_vectors[current_index]
        for student_b, text_vector_b in new_vectors:
            sim_score = similarity(text_vector_a, text_vector_b)[0][1]
            student_pair = sorted((student_a, student_b))
            score = (student_pair[0], student_pair[1], sim_score)
            if score not in plagiarism_results_set:
                plagiarism_results_set.add(score)
                score = {'firstDoc':student_pair[0], 'secondDoc':student_pair[1], 'score': sim_score }
                plagiarism_results.append(score)
    return plagiarism_results

data = check_plagiarism()
y = json.dumps(data)
print(y)
