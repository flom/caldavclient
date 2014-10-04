__author__ = 'florian'


def authorized(user, collection, permission):
    if not user:
        return False
    user_id, user_name = user.split('/')
    collection_id, collection_name = collection.name.split('/')
    return user_id == collection_id
